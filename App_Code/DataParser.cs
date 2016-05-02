
using System;


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

    public DataRecordItem()
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
        for (int k = 0; k < opts.Length; k++)
        {
            int pos;
            if ((pos = data.IndexOf(opts[k])) != -1)
            {
                return new StringHitPos(pos, opts[k].Length);
            }
        }

        return new StringHitPos(-1, 0);
    }

    public static StringHitPos LastIndexOfAny(string data, string anyOf, int startPos)
    {
        string[] opts = anyOf.Split('|');
        for (int k = 0; k < opts.Length; k++)
        {
            int pos;
            if ((pos = data.LastIndexOf(opts[k], startPos)) != -1)
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
    public static string GetTelephone(string data, int pos)
    {
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

    public static string Extract(string data, string beginExpr, string endExpr, bool optional = false)
    {
        StringHitPos pos1, pos2;

        pos1 = IndexOfAny(data, beginExpr);
        if (endExpr[0] == '^')
            pos2 = LastIndexOfAny(data, endExpr, pos1.pos);
        else pos2 = IndexOfAny(data, endExpr, pos1.pos);

        if (pos1.pos == -1 || pos2.pos == -1
            || pos1.pos + pos1.hitlen > pos2.pos)
        {
            if (optional == true)
                return "";

            throw new ArgumentException("未能解析表达式：{" + beginExpr + "," + endExpr + "}，请确定输入是否符合常用格式。");
        }
        return data.Substring(pos1.pos + pos1.hitlen, pos2.pos).Trim();
    }
}

public class OpeningParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        StringHitPos pos1, pos2;

        pos2 = IndexOfAny(data, "本院受理|本委受理");
        if (pos2.pos == -1)
            return false;

        dri.accused = data.Substring(0, pos2.pos).Trim().Replace("：", "");

        dri.accuser = Extract(data, "本院受理原告|本委受理原告|本院受理|本委受理", "诉|与");
        pos1 = IndexOfAny(dri.accuser, "原告");
        if (pos1.pos != -1)
            dri.accuser = dri.accuser.Substring(pos1.pos + pos1.hitlen);

        dri.case_title = Extract(data, "诉你们|诉你|及你们|与你司|诉", "一案", true);

        dri.court_room = Extract(data, "开庭审理|公开审理", "^在");

        char[] delimiterChars = { '。', '\n', '.' };
        pos2.pos = data.Length;
        pos1 = LastIndexOfAny(data, delimiterChars, pos2.pos - 2); // TODO: LastIndexOfAny
        dri.court = Substring(data, pos1.pos + 1, pos2.pos).Trim().Replace("\n", "");

        pos1 = IndexOfAny(data, "联系电话：|联系电话:|联系电话|电话：|电话:");
        if (pos1.pos != -1)
            dri.telephone = GetTelephone(data, pos1.pos + pos1.hitlen);

        dri.type = NoticeType.OPENING;

        return true;
    }
}

public class UnknownParser : DataParser
{

}