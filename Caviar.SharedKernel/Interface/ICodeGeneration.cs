using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Caviar.SharedKernel
{
    public interface ICodeGeneration
    {
        /// <summary>
        /// 获取反射数据
        /// </summary>
        /// <returns></returns>
        public List<ViewModelFields> GetViewModelHeaders(string name);
        /// <summary>
        /// 代码写到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="outName"></param>
        /// <param name="content"></param>
        /// <param name="isCover"></param>
        public void WriteCodeFile(string path,string outName,string content,bool isCover);
        /// <summary>
        /// 代码生成
        /// </summary>
        /// <param name="generate"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public List<TabItem> CodeGenerate(CodeGenerateData generate, string userName);
    }
}
