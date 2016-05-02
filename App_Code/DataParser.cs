
using System;
using System.Collections.Generic;

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
        PredefinedTypes[] types = {
            new PredefinedTypes(NoticeType.OPENING, "开庭审理|开庭受理|公开审理|开庭"),
        };

        for (int i = 0; i < types.Length; i++)
        {
            bool matched = true;
            string[] dnas = types[i].dna.Split(';');

            for (int j = 0; j < dnas.Length; j++)
            {
                if (IndexOfAny(data, dnas[j]).pos == -1)
                {
                    matched = false;
                    break;
                }
            }

            if (matched == true)
            {
                return types[i].type;
            }
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
        StringHitPos hp = LastIndexOfAny(data, "人民法院|法院");

        if (hp.pos == -1)
            throw new Exception("未能正确解析\"法院\"字段：没有找到相应的关键字。");

        char[] delimiterChars = { '。', '\n', '.', ';', '；', ',', '，' };
        int posBegin = data.LastIndexOfAny(delimiterChars, hp.pos);
        if (posBegin == -1)
            throw new Exception("未能正确解析\"法院\"字段。");

        return data.Substring(posBegin + 1, hp.pos + hp.hitlen - (posBegin + 1))
            .Trim(delimiterChars);
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

        throw new Exception("未能解析表达式：{" + beginExpr + "," + endExpr + "}，请确定输入是否符合常用格式。");
    }
}

public class OpeningParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        dri.accused = Extract(data, "本院受理原告|本委受理原告|本院受理|本委受理|本院定于|本院于", "^");
        dri.accused = dri.accused.Replace("：", "");

        dri.accuser = Extract(data, "本院受理原告|本委受理原告|本院受理|本委受理|本院定于|本院于", "诉|与");
        int pos = dri.accuser.IndexOf("原告");
        if (pos != -1)
            dri.accuser = dri.accuser.Substring(pos + 2);

        dri.case_title = Extract(data, "诉被告你们|诉被告|诉你们|诉你方|诉你|及你们|与你司|与你们|与你|诉", "一案", true);

        dri.court_room = Extract(data, "公开开庭审理|开庭审理|公开审理|开庭", "^在");

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
        throw new Exception("该类业务数据当前未能识别");
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
        StringHitPos hp0, hp1;
        int begin = 0, end;

        for (; ; )
        {
            hp0 = DataParser.IndexOfAny(data, "本院受理|本委受理|本院定于|本院于", begin);
            if (hp0.pos == -1)
                break;

            hp1 = DataParser.IndexOfAny(data, "本院受理|本委受理|本院定于|本院于", hp0.pos + hp0.hitlen);
            if (hp1.pos != -1) {
                char[] paraChars = { '\n' };
                end = data.LastIndexOfAny(paraChars, hp1.pos);
            } else { end = data.Length; }

            if (end > begin)
            {
                DataParagraph dp = new DataParagraph();
                dp.begin = begin;
                dp.end = end;
                dp.text = data.Substring(begin, end - begin);
                paragraphs.Add(dp);
            }

            begin = end;
        }

        return paragraphs.Count;
    }
}