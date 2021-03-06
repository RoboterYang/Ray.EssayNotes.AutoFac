﻿//系统包
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ray.EssayNotes.AutoFac.Infrastructure.Helpers
{
    /// <summary>
    /// 反射辅助类
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        ///  获取Asp.Net FrameWork项目所有程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetAllAssemblies()
        {
            //todo:需要当前项目引用所有程序集，待改善
            //1.获取当前程序集所有引用程序集
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            List<Assembly> assemblies = entryAssembly.GetReferencedAssemblies()
                .Select(Assembly.Load)
                .Where(m => m.FullName.Contains("Ray"))
                .ToList();
            assemblies.Add(entryAssembly);
            return assemblies.ToArray();
        }

        /// <summary>
        ///  获取托管在IIS上的Asp.Net FrameWork项目所有程序集
        /// </summary>
        /// <returns></returns>
        public static Assembly[] GetAllAssembliesWebIIS()
        {
            //应用第一次启动时IIS托管应用里面所有的程序集都被加载进 AppDomain , 但是 当AppDomain被IIS回收时, 程序集只会按需加载.
            //使用如下方法会立刻强制相关的程序集加载进 AppDomain 使其可以被用于模块扫描.
            Assembly[] assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies().Cast<Assembly>()
                .Where(x => x.FullName.Contains("Ray"))
                .ToArray();
            return assemblies;
        }
    }
}
