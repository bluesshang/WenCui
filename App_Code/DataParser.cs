
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
    CLAIMS, // �߸�
    MISSING, // ʧ��
    EXECUTE, // ִ��
    APPEAL, // ����
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
        type = NoticeType.UNKNOWN;
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
                }
                else if (dp.text.Length > 80) paragraphs.Add(dp);
            }

            begin = end + 1;
        }

        return paragraphs.Count;
    }
}
public abstract class DataParser
{
    public static string PRESET_CASE_TITLE = "��֤��ͬ����|���������|��ҵ�����ͬ����|������ͬ����|����Ӫ��ͬ����|�������޺�ͬ����|���ڽ���ͬ����|����ͬ����|��ͬ����|��ͨ�¹����ξ���";
    public static string PRESET_CASE = @"([һ�����������߰˾���]|\d+)��";
    public virtual bool Parse(string data, ref DataRecordItem dri)
    {
        string[] part1 = {
            @"(?<accused>.+?)[��:]*(��Ժ����ԭ��|��ί����ԭ��|��Ժ������|��Ժ����|��Ժ����|��ί����|ԭ��)(?<accuser>.+?)(�߱�������|�߱���|���㷽|������|�뱻��|����˾|������|���㼰|���㼰|������|����|����|��|��)(?<case_title>.+?)([һ�����������߰˾���]|\d+)��",
            @"(��Ժ����|��Ժ��)(.+)ԭ��(?<accuser>.+?)(�߱���|���㷽|������|����|��)(?<accused>.+?)(?<case_title>" + PRESET_CASE_TITLE + @")*" + PRESET_CASE,
            @"(?<accused>.+?)[��:]*��Ժ����(?<accuser>.+?)����(?<case_title>.+?)" + PRESET_CASE,
            @"(?<accused>.+?)[��:]*��Ժ����(.+?)ԭ��(?<accuser>.+?)(�߱���|���㷽|������|����|��)(.+)(?<case_title>" + PRESET_CASE_TITLE + ")",
            @"(?<accused>.+?)[��:]*��Ժ����(.+?)(ԭ��|��(.+?)��������|��������)(?<accuser>.+?)(�߱���|���㷽|������|����|��)(?<case_title>.+?)" + PRESET_CASE,
            @"(?<accused>.+?)[��:](?<accuser>.+?)(�ֱ�����|�߱���|���㷽|������|����|����|��)(?<case_title>.+?)" + PRESET_CASE,
        };

        Match m = Expr(data, part1);
        if (m != null)
        {
            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            dri.case_title = m.Groups["case_title"].Value;
            // simplify the case title if any
            SimplfyCaseTitle(data, ref dri);
        }

        int t = dri.accuser.IndexOf("ԭ��");
        if (t != -1)
            dri.accuser = dri.accuser.Substring(t + 2);

        dri.court_room = ExtractCourtRoom(data);

        dri.court = ExtractCourt(data);

        dri.telephone = ExtractTelephone(data);

        return false;
    }

    public static void SimplfyCaseTitle(string data, ref DataRecordItem dri)
    {
        if (dri.case_title == "")
            dri.case_title = data; // guess from the whole string

        int pos = dri.case_title.IndexOf(dri.accused);
        if (pos != -1)
            dri.case_title = dri.case_title.Substring(pos + dri.accused.Length);
        StringHitPos hp = IndexOfAny(dri.case_title, PRESET_CASE_TITLE);
        if (hp.pos != -1)
        {   // use the preset case title if any
            dri.case_title = dri.case_title.Substring(hp.pos, hp.hitlen);
        }
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
            new PredefinedTypes(NoticeType.OPENING, @"(��ͥ����|��ͥ����|��������|���ڽ������о�|��(.+)��ͥ[.��;��,��)��(��]|��ͥʱ��(.+)(��ͥ�ص�|�ص�)(.+))"),
            new PredefinedTypes(NoticeType.CLAIMS, @"(��ʾ�߸�|�߸�)"),
            new PredefinedTypes(NoticeType.MISSING, @"(����(.+)ʧ��)"),
            new PredefinedTypes(NoticeType.EXECUTE, @"(����ִ��|����ִ��)"),
            new PredefinedTypes(NoticeType.APPEAL, @"(������(.+?)��������|�о���(.+)�������|��(.+?)�����ð��о�(.+)�������)"),
            new PredefinedTypes(NoticeType.SENTENCE, @"(�������о�|���������о�|�粻���о�|�о���)"),
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
            case NoticeType.SENTENCE:
                return new SentenceParser();
            case NoticeType.CLAIMS:
                return new ClaimsParser();
            case NoticeType.MISSING:
                return new MissingParser();
            case NoticeType.EXECUTE:
                return new ExecuteParser();
            case NoticeType.APPEAL:
                return new AppealParser();

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
        Regex reg = new Regex(@"�绰[:����Ϊ]*(?<telphone>.+?)[.;����)��]");
        Match m = reg.Match(data);
        //for (int i = 0; i < m.Groups.Count; i++)
        //{ }
        return m.Groups["telphone"].Value;
        /*StringHitPos posHit = IndexOfAny(data, "��ϵ�绰��|��ϵ�绰:|��ϵ�绰|�绰��|�绰:");

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
        return tel;*/
    }

    public static string ExtractCourt(string data)
    {
        char[] delimiterChars = { '\n', '��', '.', ';', '��', ',', '��' };
        int posBegin = data.LastIndexOfAny(delimiterChars);
        if (posBegin == -1)
            throw new DataParseException("δ����ȷ����\"��Ժ\"�ֶΡ�");

        StringHitPos hp = IndexOfAny(data, "����Ժ|ίԱ��|��Ժ", posBegin + 1);
        if (hp.pos == -1)
            throw new DataParseException("δ����ȷ����\"��Ժ\"�ֶΣ�û���ҵ���Ӧ�Ĺؼ��֡�");

        return data.Substring(posBegin + 1, hp.pos + hp.hitlen - (posBegin + 1))
            .Trim(delimiterChars);
    }

    public static string ExtractCourtRoom(string data)
    {
        string[] exprs = {
            "��(?<court_room>.+?)(������ͥ����|��ͥ����|��������|��ͥ)",
            "��ͥʱ��(.+?)�ص�(��|Ϊ)*[:��]*(?<court_room>.+?)[��.;����)]"
        };
        Match m = Expr(data, exprs);
        if (m == null)
            return "";
        return m.Groups["court_room"].Value;//.TrimEnd(trimChars).TrimStart(trimChars);
    }
    public static Match Expr(string data, string[] exprs)
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
}

public class OpeningParser : DataParser
{

    public override bool Parse(string data, ref DataRecordItem dri)
    {
        bool ret = base.Parse(data, ref dri);

        dri.type = NoticeType.OPENING;

        return true;
    }
}

public class SentenceParser : DataParser
{

    public override bool Parse(string data, ref DataRecordItem dri)
    {
        bool ret = base.Parse(data, ref dri);

        dri.type = NoticeType.SENTENCE;

        return true;
    }
}

public class ClaimsParser : DataParser
{

    public override bool Parse(string data, ref DataRecordItem dri)
    {
        //bool ret = base.Parse(data, ref dri);
        string[] part1 = {
            @"������(?<accuser>.+?)(��)*(��ʾ�߸�����|�߸�����|����)",
            @"������(?<accuser>.+?)��(.+?)����",
        };

        Match m = Expr(data, part1);

        dri.accused = "��ʾ�߸�";
        dri.accuser = m.Groups["accuser"].Value;
        dri.court = ExtractCourt(data);
        dri.telephone = ExtractTelephone(data);

        dri.type = NoticeType.CLAIMS;

        return true;
    }
}

public class MissingParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        //bool ret = base.Parse(data, ref dri);
        string[] part1 = {
            @"(?<accuser>.+?)(��������|����)(?<accused>.+?)ʧ��",
        };

        Match m = Expr(data, part1);

        dri.accused = m.Groups["accused"].Value;
        dri.accuser = m.Groups["accuser"].Value;
        dri.court = ExtractCourt(data);
        dri.telephone = ExtractTelephone(data);

        dri.type = NoticeType.MISSING;

        return true;
    }
}
public class ExecuteParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        //bool ret = base.Parse(data, ref dri);
        string[] part1 = {
            @"(?<accused>.+?)[:��]*��Ժ����(����ִ����)*(?<accuser>.+?)����ִ��(?<case_title>.+?)һ��",
            @"����ִ����(?<accuser>.+?)�뱻ִ����(?<accused>.+?)(?<case_title>" + PRESET_CASE_TITLE + ")",
        };

        Match m = Expr(data, part1);
        if (m != null)
        {
            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            dri.case_title = m.Groups["case_title"].Value;
            SimplfyCaseTitle(data, ref dri);
        }
        dri.court = ExtractCourt(data);
        dri.telephone = ExtractTelephone(data); 
        
        dri.type = NoticeType.EXECUTE;

        return true;
    }
}
public class AppealParser : DataParser
{
    public override bool Parse(string data, ref DataRecordItem dri)
    {
        //bool ret = base.Parse(data, ref dri);
        string[] part1 = {
            @"(?<accused>.+?)[:��]������(?<accuser>.+?)��(?<case_title>.+?)��������",
            @"(?<accused>.+?)[:��](.+)������(?<accuser>.+?)��(?<case_title>.+?)��������",
            @"(?<accused>.+?)[:��](.+)�о��󱻸�(?<accuser>.+?)�������",
            @"(?<accused>.+?)[:��](.+)��(?<accuser>.+?)�����ð��о�(.+)�������"
        };

        Match m = Expr(data, part1);
        if (m != null)
        {
            dri.accused = m.Groups["accused"].Value;
            dri.accuser = m.Groups["accuser"].Value;
            dri.case_title = m.Groups["case_title"].Value;
            SimplfyCaseTitle(data, ref dri);
        }
        dri.court = ExtractCourt(data);
        dri.telephone = ExtractTelephone(data); 
        
        dri.type = NoticeType.APPEAL;

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

