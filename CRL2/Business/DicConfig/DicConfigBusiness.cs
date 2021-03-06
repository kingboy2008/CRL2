﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace CRL.DicConfig
{
    /// <summary>
    /// 字典设置维护
    /// 通过TYPE区分不同的用途
    /// </summary>
    public class DicConfigBusiness<TType> : BaseProvider<IDicConfig> where TType : class
    {
        public static DicConfigBusiness<TType> Instance
        {
            get { return new DicConfigBusiness<TType>(); }
        }
        protected override DBExtend dbHelper
        {
            get { return GetDbHelper<TType>(); }
        }
        /// <summary>
        /// 添加一项
        /// </summary>
        /// <param name="dic"></param>
        /// <returns></returns>
        public int Add(IDicConfig dic)
        {
            if (QueryItem(b => b.Name == dic.Name && b.DicType == dic.DicType) != null)
            {
                return 0;
            }
            int id = base.Add(dic);
            //ClearCache();
            return id;
        }
        /// <summary>
        /// 取名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetName(int id)
        {
            DicConfig dic = Get(id);
            if (dic == null)
                return "";
            return dic.Name;
        }
        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetValue(int id)
        {
            DicConfig dic = Get(id);
            if (dic == null)
                return "";
            return dic.Value;
        }
        /// <summary>
        /// 取对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IDicConfig Get(int id, bool noCache = false)
        {
            if (noCache)
            {
                return QueryItem(b => b.Id == id);
            }
            else
            {
                var items = AllCache.Where(b => b.Id == id);
                return items.FirstOrDefault();
            }
        }
        /// <summary>
        /// 取该类型的对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<IDicConfig> Get(string type, bool noCache = false)
        {
            if (noCache)
            {
                return QueryList(b => b.DicType == type);
            }
            return AllCache.Where(b => b.DicType == type).ToList();
        }
        public string GetName(Enum type, string value)
        {
            return GetName(type.ToString(), value);
        }
        public string GetName(string type, string value)
        {
            var dic = QueryItemFromAllCache(b => b.DicType == type && b.Value == value);
            if (dic == null)
                return "";
            return dic.Name;

        }
        public List<CRL.DicConfig.IDicConfig> Get(Enum dType)
        {
            return Get(dType.ToString());
        }
        public CRL.DicConfig.IDicConfig Get(Enum dType, string name, bool nocache = false)
        {
            return Get(dType.ToString(), name, nocache);
        }
        public CRL.DicConfig.IDicConfig Get(string type, string dName, bool noCache = false)
        {
            IDicConfig dic;
            if (noCache)
            {
                dic = QueryItem(b => b.DicType == type && b.Name == dName);
            }
            else
            {
                dic = AllCache.Where(b => b.DicType == type && b.Name == dName).FirstOrDefault();
            }
            if (dic == null)
            {
                throw new Exception(string.Format("找不到对应的字典 类型:{0} 名称:{1}", type, dName));
            }
            return dic;
        }
        /// <summary>
        /// 获取对象分组
        /// </summary>
        /// <returns></returns>
        public List<string> GetTypeGroup()
        {
            List<string> list = new List<string>();
            foreach (IDicConfig v in AllCache)
            {
                if (!list.Contains(v.DicType))
                {
                    list.Add(v.DicType);
                }
            }
            return list;
        }
        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Update(int id,string name,string value,string remark="")
        {
            CRL.ParameCollection c = new ParameCollection();
            c["name"] = name;
            c["value"] = value;
            if (!string.IsNullOrEmpty(remark))
            {
                c["remark"] = remark;
            }
            Update(b => b.Id == id, c);
            //ClearCache();
            return true;
        }
    }
}
