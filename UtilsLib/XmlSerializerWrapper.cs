using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace UtilsLib
{
    /// <summary>
    /// 包装 Xml 序列化器功能，提供实体与持久化文本间的快速访问。
    /// </summary>
    /// <typeparam name="T">可以序列化的类型。</typeparam>
    public class XmlSerializerWrapper<T>
        where T : class, new()
    {
        XmlSerializer xmlSerializer;
        string filePath;

        /// <summary>
        /// 获取持久化文本中表示的可序列化实体。
        /// </summary>
        public T Entity { get; private set; }

        /// <summary>
        /// 初始化一个 Xml 序列化器包装，使用默认持久化文本路径。
        /// </summary>
        public XmlSerializerWrapper()
            : this(AppDomain.CurrentDomain.BaseDirectory + typeof(T).FullName + ".xml")
        { }
        /// <summary>
        /// 使用指定的持久化文本路径初始化一个 Xml 序列化器包装。
        /// </summary>
        public XmlSerializerWrapper(string filePath)
        {
            this.xmlSerializer = new XmlSerializer(typeof(T));
            this.filePath = filePath;

            if (File.Exists(this.filePath))
            {
                this.Load();
            }
            else
            {
                this.Entity = new T();
                this.Save();
            }
        }

        /// <summary>
        /// 从持久化文本中加载属性到实体。
        /// </summary>
        void Load()
        {
            using (FileStream stream = new FileStream(this.filePath, FileMode.Open))
            {
                this.Entity = this.xmlSerializer.Deserialize(stream) as T;
            }
        }
        /// <summary>
        /// 保存实体的属性到持久化文本。
        /// </summary>
        public void Save()
        {
            using (XmlTextWriter writer = new XmlTextWriter(this.filePath, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;

                this.xmlSerializer.Serialize(writer, this.Entity);
            }
        }
    }
}
