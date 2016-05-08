
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
    OPENING, // ��ͥ
    SENTENCE, // �о�
    CORRECTION, // ����
    UNKNOWN
}

// ҵ����Ϣ
public class BizRecordItem
{
    public int id;
    public double receivable; // Ӧ�տ�
    public double payed; // ʵ�տ�
    public int date; // ʵ���տ�����
    public string payee; // ʵ���տ���
}
public class DataRecordItem
{
    public int id;
    public int date; // ����
    public string accused; // ����
    public string accuser; // ԭ��
    public string court; // ��Ժ
    public string court_room; // ��ͥ
    public NoticeType type;
    public string judge; // ����
    public string telephone;
    public string clerk; // ҵ��Ա
    public string memo;
    public string case_title; // ��������

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
            throw new DataParseException(ParseError.TooShort, "��Ϣ̫�٣��޷��Զ�������");

        PredefinedTypes[] types = {
            new PredefinedTypes(NoticeType.OPENING, "��ͥ����|��ͥ����|��������"),
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
        StringHitPos posHit = IndexOfAny(data, "��ϵ�绰��|��ϵ�绰:|��ϵ�绰|�绰��|�绰:");

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
            else if ((c == '-' || c == '��' || c == ';' || c == '��')
                && isdigital(t[pos - 1]) && isdigital(t[pos + 1]))
                tel += c;
            else break;
        }
        return tel;
    }

    public static string ExtractCourt(string data)
    {
        StringHitPos hp = LastIndexOfAny(data, "����Ժ|ίԱ��|��Ժ");

        if (hp.pos == -1)
            throw new DataParseException("δ����ȷ����\"��Ժ\"�ֶΣ�û���ҵ���Ӧ�Ĺؼ��֡�");

        char[] delimiterChars = { '��', '\n', '.', ';', '��', ',', '��' };
        int posBegin = data.LastIndexOfAny(delimiterChars, hp.pos);
        if (posBegin == -1)
            throw new DataParseException("δ����ȷ����\"��Ժ\"�ֶΡ�");

        return data.Substring(posBegin + 1, hp.pos + hp.hitlen - (posBegin + 1))
            .Trim(delimiterChars);
    }

    public static string ExtractCourtRoom(string data)
    {
        string court_room = ExtractExpr(data, "��(.+?)(������ͥ����|��ͥ����|��������|��ͥ)", 1);
        if (court_room == "")
        {
            court_room = ExtractExpr(data, "��ͥʱ��(.+?)(�ص���|�ص�Ϊ|�ص�)(.+?)[��|.|;|��|��|)]", 3);
        }

        //char[] trimChars = { '(', '��', ')', '��', ':', '��' };
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

        throw new DataParseException(ParseError.Unrecognize, "δ�ܽ������ʽ��{" + beginExpr + "," + endExpr + "}����ȷ�������Ƿ���ϳ��ø�ʽ��");
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
            "(?<accused>.+)(��|:)(��Ժ����ԭ��|��ί����ԭ��|��Ժ������|��Ժ����|��Ժ����|��ί����|ԭ��)(?<accuser>.+?)(�߱�������|�߱���|���㷽|������|����|����˾|������|���㼰|����|��)(?<case_title>.+?)һ��",
            "(?<accused>.+)(��|:)(?<accuser>.+?)(���㷽|������|����|��)(?<case_title>.+?)һ��",
            "(��Ժ����|��Ժ��)(.+)ԭ��(?<accuser>.+)(�߱���|���㷽|������|����|��)(?<accused>.+)һ��",
            "(?<accused>.+)(��|:)��Ժ����(?<accuser>.+?)����(?<case_title>.+?)һ��",
        };

        Match m = Expr(data, part1);
        if (m != null)
        {
            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            //if (dri.accused == "" || dri.accuser == "")
            //    throw new DataParseException(ParseError.GeneralError, "δ���Զ�����\"ԭ��\"��\"������Ϣ\"");
            dri.case_title = m.Groups["case_title"].Value;
        }
        /*
        try
        {
            dri.accused = Extract(data, "��Ժ����ԭ��|��ί����ԭ��|��Ժ������|��Ժ����|��Ժ����|��ί����|��Ժ����|��Ժ��", "^");
            dri.accused = dri.accused.Replace("��", "");

            dri.accuser = Extract(data, "��Ժ����ԭ��|��ί����ԭ��|��Ժ������|��Ժ����|��Ժ����|��ί����|��Ժ����|��Ժ��", "��|��");

            dri.case_title = Extract(data, "�߱�������|�߱���|������|���㷽|����|������|����˾|������|����|��", "һ��", true);
        }
        catch (DataParseException e)
        {
            Regex reg = new Regex("(?<accused>.+)(��|:)(?<accuser>.+?)(���㷽|������|����|��)(?<case_title>.+?)һ��");

            Match m = reg.Match(data);
            if (m.Groups.Count < 6)
                throw e;

            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            dri.case_title = m.Groups["case_title"].Value;
        }*/

        int t = dri.accuser.IndexOf("ԭ��");
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
        throw new DataParseException(ParseError.NotSupport, "����ҵ�����ݵ�ǰδ��ʶ��");
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
                    && DataParser.LastIndexOfAny(dp.text, "����Ժ|ίԱ��|��Ժ").pos != -1)
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