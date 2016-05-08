
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum ParseError
{
    Okay = 0,
    GeneralError = 1,
    TooShort = 2,
    NotSupport = 3,
    Unrecognize = 4
}
public class DataParseException : Exception
{
    public ParseError code;
    public DataParseException(ParseError code, string message)
        :base(message)
    {
        this.code = code;
    }
    public DataParseException(string message)
        :base(message)
    {
        this.code = ParseError.GeneralError;
    }
}
public enum NoticeType : int
{
    OPENING, // 开庭
    SENTENCE, // 判决
    CORRECTION, // 更正
    UNKNOWN
}

// 业务信息
public class BizRecordItem
{
    public int id;
    public double receivable; // 应收款
    public double payed; // 实收款
    public int date; // 实际收款日期
    public string payee; // 实际收款人
}
public class DataRecordItem
{
    public int id;
    public int date; // 日期
    public string accused; // 被告
    public string accuser; // 原告
    public string court; // 法院
    public string court_room; // 法庭
    public NoticeType type;
    public string judge; // 法官
    public string telephone;
    public string clerk; // 业务员
    public string memo;
    public string case_title; // 案件名称

    public void Reset()
    {
        accused = "";
        accuser = "";
        court = "";
        court_room = "";
        judge = "";
        telephone = "";
        clerk = "";
        memo = "";
        case_title = "";
    }
    public void MakeJson()
    {
        accused.Replace("\n", "<br>");
    }
    public DataRecordItem()
    {
        Reset();
    }
}

struct PredefinedTypes
{
    public NoticeType type;
    public string dna;

    public PredefinedTypes(NoticeType type, string dna)
    {
        this.type = type;
        this.dna = dna;
    }
};

public class StringHitPos
{
    public int pos, hitlen;
    public StringHitPos(int pos, int hitlen)
    {
        this.pos = pos;
        this.hitlen = hitlen;
    }
};
public abstract class DataParser
{
    public virtual bool Parse(string data, ref DataRecordItem dri)
    {
        return false;
    }

    public static StringHitPos IndexOfAny(string data, string anyOf, int startPos = 0)
    {
        string[] opts = anyOf.Split('|');
        List<StringHitPos> hitPos = new List<StringHitPos>(opts.Length);

        for (int k = 0; k < opts.Length; k++)
        {
            int pos;
            if ((pos = data.IndexOf(opts[k], startPos)) != -1)
            {
                hitPos.Add(new StringHitPos(pos, opts[k].Length));
                //return new StringHitPos(pos, opts[k].Length);
            }
        }

        if (hitPos.Count == 0)
            return new StringHitPos(-1, 0);

        int bestPos = data.Length, selHitPos = -1;
        for (int i = 0; i < hitPos.Count; i++)
        {
            if (hitPos[i].pos < bestPos)
            {
                selHitPos = i;
                bestPos = hitPos[i].pos;
            }
        }

        return new StringHitPos(hitPos[selHitPos].pos, hitPos[selHitPos].hitlen);
    }

    public static StringHitPos LastIndexOfAny(string data, string anyOf, int startPos = -1)
    {
        string[] opts = anyOf.Split('|');
        for (int k = 0; k < opts.Length; k++)
        {
            int pos;
            if (startPos == -1)
                pos = data.LastIndexOf(opts[k]);
            else pos = data.LastIndexOf(opts[k], startPos);

            if (pos != -1)
            {
                return new StringHitPos(pos, opts[k].Length);
            }
        }

        return new StringHitPos(-1, 0);
    }

    public static StringHitPos LastIndexOf(string data, string value, int startIndex)
    {
        return new StringHitPos(data.LastIndexOf(value, startIndex), value.Length);
    }
    public static StringHitPos LastIndexOfAny(string data, char[] value, int startIndex)
    {
        return new StringHitPos(data.LastIndexOfAny(value, startIndex), value.Length);
    }
    /*public static string Substring(string data, int pos1, int pos2)
    {
        return data.Substring(pos1, pos2 - pos1);
    }*/

    public static NoticeType GetNoticeType(string data)
    {
        if (data.Length < 50)
            throw new DataParseException(ParseError.TooShort, "信息太少，无法自动解析。");

        PredefinedTypes[] types = {
            new PredefinedTypes(NoticeType.OPENING, "(开庭审理|开庭受理|公开审理|在(.+)开庭[.。;；,，])"),
        };

        for (int i = 0; i < types.Length; i++)
        {
            // bool matched = false;
            //string[] dnas = types[i].dna.Split('|');

            //for (int j = 0; j < dnas.Length; j++)
            //{
            Regex reg = new Regex(types[i].dna);

                Match m = reg.Match(data);

                //if (IndexOfAny(data, dnas[j]).pos == -1)
                if (m.Groups.Count > 1)
                {
                    return types[i].type;
                }
            //}

            //if (matched == true)
            //{
            //    return types[i].type;
            //}
        }

        return NoticeType.UNKNOWN;
    }
    public static DataParser GetParser(string data)
    {
        NoticeType type = GetNoticeType(data);

        switch (type)
        {
            case NoticeType.OPENING:
                return new OpeningParser();

            case NoticeType.UNKNOWN:
            default:
                return new UnknownParser();
        }
    }

    public static bool isdigital(char c)
    {
        return c >= '0' && c <= '9' ? true : false;
    }
    public static string ExtractTelephone(string data)
    {
        StringHitPos posHit = IndexOfAny(data, "联系电话：|联系电话:|联系电话|电话：|电话:");

        if (posHit.pos == -1)
            return "";

        int pos = posHit.pos;
        string tel = "";
        string t = data.Substring(pos).Replace(" ", "");

        pos = 0;
        char c;
        int end = t.Length - 1;
        for (; pos < end; pos++)
        {
            c = t[pos];
            if (isdigital(c))
                tel += c;
            else if ((c == '-' || c == '、' || c == ';' || c == '；')
                && isdigital(t[pos - 1]) && isdigital(t[pos + 1]))
                tel += c;
            else break;
        }
        return tel;
    }

    public static string ExtractCourt(string data)
    {
        StringHitPos hp = LastIndexOfAny(data, "人民法院|委员会|法院");

        if (hp.pos == -1)
            throw new DataParseException("未能正确解析\"法院\"字段：没有找到相应的关键字。");

        char[] delimiterChars = { '。', '\n', '.', ';', '；', ',', '，' };
        int posBegin = data.LastIndexOfAny(delimiterChars, hp.pos);
        if (posBegin == -1)
            throw new DataParseException("未能正确解析\"法院\"字段。");

        return data.Substring(posBegin + 1, hp.pos + hp.hitlen - (posBegin + 1))
            .Trim(delimiterChars);
    }

    public static string ExtractCourtRoom(string data)
    {
        string court_room = ExtractExpr(data, "在(.+?)(公开开庭审理|开庭审理|公开审理|开庭)", 1);
        if (court_room == "")
        {
            court_room = ExtractExpr(data, "开庭时间(.+?)(地点是|地点为|地点)(.+?)[。|.|;|；|）|)]", 3);
        }

        //char[] trimChars = { '(', '（', ')', '）', ':', '：' };
        return court_room;//.TrimEnd(trimChars).TrimStart(trimChars);
    }
    public static string ExtractExpr(string data, string expr, int idx)
    {
        Regex reg = new Regex(expr);

        Match m = reg.Match(data);
        if (m.Groups.Count > idx)
            return m.Groups[idx].Value;

        return "";
    }
    public static string Extract(string data, string beginExpr, string endExpr, bool optional = false)
    {
        StringHitPos pos1, pos2;
        int begin, end, offset = 0;
        int keypos = 0;

        for (; ; )
        {
            pos1 = IndexOfAny(data, beginExpr, keypos);
            if (pos1.pos == -1)
                break;
            keypos = pos1.pos + pos1.hitlen;

            if (endExpr[0] == '^')
            {   // backforward match
                string endExprTmp = endExpr.Substring(1); // trim the '^'
                if (endExprTmp.Length == 0)
                    begin = 0;
                else
                {
                    pos2 = LastIndexOfAny(data, endExprTmp, pos1.pos);
                    begin = pos2.pos;
                    offset = pos2.hitlen;
                }
                end = pos1.pos;
            }
            else
            {
                pos2 = IndexOfAny(data, endExpr, pos1.pos);
                begin = pos1.pos;
                offset = pos1.hitlen;
                end = pos2.pos;
            }

            if (begin != -1 && end != -1
                && begin + offset <= end)
            {
                return data.Substring(begin + offset, end - (begin + offset)).Trim();
            }
        }

        if (optional == true)
            return "";

        throw new DataParseException(ParseError.Unrecognize, "未能解析表达式：{" + beginExpr + "," + endExpr + "}，请确定输入是否符合常用格式。");
    }
}

public class OpeningParser : DataParser
{
    Match Expr(string data, string [] exprs)
    {
        for (int i = 0; i < exprs.Length; i++)
        {
            Regex reg = new Regex(exprs[i]);
            Match m = reg.Match(data);
            if (m.Groups.Count > 1)
                return m;
        }

        return null;
    }
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        string[] part1 = {
            @"(?<accused>.+)(：|:)(本院受理原告|本委受理原告|本院已受理|本院受理|我院受理|本委受理|原告)(?<accuser>.+?)(诉被告你们|诉被告|诉你方|诉你们|与被告|与你司|与你们|与你及|诉你及|诉你与|诉你|与你|诉|与)(?<case_title>.+?)([一二三四五六七八九]|\d+)案",
            @"(?<accused>.+)(：|:)(?<accuser>.+?)(诉你方|诉你们|诉你|与你|诉)(?<case_title>.+?)([一二三四五六七八九]|\d+)案",
            @"(本院定于|本院于)(.+)原告(?<accuser>.+)(诉被告|诉你方|诉你们|诉你|诉)(?<accused>.+)([一二三四五六七八九]|\d+)案",
            @"(?<accused>.+)(：|:)本院受理(?<accuser>.+?)及你(?<case_title>.+?)([一二三四五六七八九]|\d+)案",
            @"(?<accused>.+)(：|:)本院定于(.+?)原告(?<accuser>.+?)诉被告(.+)(?<case_title>保证合同纠纷|民间借贷纠纷|物业服务合同纠纷|买卖合同纠纷)"
        };

        Match m = Expr(data, part1);
        if (m != null)
        {
            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            //if (dri.accused == "" || dri.accuser == "")
            //    throw new DataParseException(ParseError.GeneralError, "未能自动解析\"原告\"或\"被告信息\"");
            dri.case_title = m.Groups["case_title"].Value;
        }
        /*
        try
        {
            dri.accused = Extract(data, "本院受理原告|本委受理原告|本院已受理|本院受理|我院受理|本委受理|本院定于|本院于", "^");
            dri.accused = dri.accused.Replace("：", "");

            dri.accuser = Extract(data, "本院受理原告|本委受理原告|本院已受理|本院受理|我院受理|本委受理|本院定于|本院于", "诉|与");

            dri.case_title = Extract(data, "诉被告你们|诉被告|诉你们|诉你方|诉你|及你们|与你司|与你们|与你|诉", "一案", true);
        }
        catch (DataParseException e)
        {
            Regex reg = new Regex("(?<accused>.+)(：|:)(?<accuser>.+?)(诉你方|诉你们|诉你|诉)(?<case_title>.+?)一案");

            Match m = reg.Match(data);
            if (m.Groups.Count < 6)
                throw e;

            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            dri.case_title = m.Groups["case_title"].Value;
        }*/

        int t = dri.accuser.IndexOf("原告");
        if (t != -1)
            dri.accuser = dri.accuser.Substring(t + 2);

        dri.court_room = ExtractCourtRoom(data);

        dri.court = ExtractCourt(data);

        dri.telephone = ExtractTelephone(data);

        dri.type = NoticeType.OPENING;

        return true;
    }
}

public class UnknownParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        throw new DataParseException(ParseError.NotSupport, "该类业务数据当前未能识别");
    }
}

public class DataParagraph
{
    public string text;
    public int begin, end;
};

public class DataParagrapher
{
    //public DataParagraph[] paragraphs;
    public List<DataParagraph> paragraphs = new List<DataParagraph>();

    public int DoParagraph(string data)
    {
        int begin = 0, end;

        data = data + '\n';

        for (; ; )
        {
            end = data.IndexOf('\n', begin);
            if (end == -1)
                break;

            if (end > begin)
            {
                DataParagraph dp = new DataParagraph();
                dp.begin = begin;
                dp.end = end;
                dp.text = data.Substring(begin, end - begin)
                    .Replace("\r", "");
                if (paragraphs.Count > 0
                    && dp.text.Length < 100
                    && DataParser.LastIndexOfAny(dp.text, "人民法院|委员会|法院").pos != -1)
                {
                    paragraphs[paragraphs.Count - 1].text =
                        paragraphs[paragraphs.Count - 1].text + "\n" + dp.text;
                } else paragraphs.Add(dp);
            }

            begin = end + 1;
        }

        return paragraphs.Count;
    }
}