using System;
using System.Collections.Generic;
using System.Text;

namespace EcardQuery
{
    static class Serializer
    {
        public static StringBuilder getCsv(IEnumerable<TransactionData> source)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("时间,交易类型,子系统名称,交易额,账户余额,卡内余额,次数");
            foreach(var item in source)
            {
                builder.AppendFormat(
                    "{0},{1},{2},{3},{4},{5},{6}\n",
                    getCsvString( item.DateTime),
                    getCsvString(item.TranscationType),
                    getCsvString(item.SubSystem),
                    getCsvString(item.Delta),
                    getCsvString(item.AccountBalance),
                    getCsvString(item.CardBalance),
                    getCsvString(item.Id));
            }
            return builder;
        }

        static string getCsvString(object source)
        {
            return string.Format("\"{0}\"", source.ToString().Replace("\"", "\"\""));
        }
    }
}
