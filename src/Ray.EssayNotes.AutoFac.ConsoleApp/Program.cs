﻿//系统包
using System;
//三方包
using Autofac;
using Ray.EssayNotes.AutoFac.ConsoleApp.Test;
using Ray.EssayNotes.AutoFac.ConsoleApp.Test.TestLifetimeScope;
using Ray.EssayNotes.AutoFac.ConsoleApp.Test.TestRegister;
//本地项目包
using Ray.EssayNotes.AutoFac.Domain.Entity;
using Ray.EssayNotes.AutoFac.Infrastructure.Ioc;
using Ray.EssayNotes.AutoFac.Service.IAppService;

namespace Ray.EssayNotes.AutoFac.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestRegister();

            TestLifetimeScope();
        }

        private static void TestRegister()
        {
            while (true)
            {
                Console.WriteLine("\r\n请输入【注册】测试编号(01-12)：");

                string testNum = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(testNum)) continue;
                ITest test = TestRegisterFactory.Create(testNum);
                test.Run();
            }
        }

        private static void TestLifetimeScope()
        {
            while (true)
            {
                Console.WriteLine("\r\n请输入【生命周期】测试编号(01-12)：");

                string testNum = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(testNum)) continue;
                ITest test = TestLifetimeScopeFactory.Create(testNum);
                test.Run();
            }
        }


        /// <summary>
        /// 生命周期——瞬时实例
        /// </summary>
        public static void TestInstancePerDependency()
        {
            //注册
            ConsoleContainer.Init(x =>
                {
                    x.RegisterType<StudentEntity>().InstancePerDependency();
                    return x;
                }
            );
            using (var scope = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                var stu1 = scope.Resolve<StudentEntity>();
                Console.WriteLine($"第1次打印：{stu1.Name}");
                stu1.Name = "张三";
                Console.WriteLine($"第2次打印：{stu1.Name}");

                var stu2 = scope.Resolve<StudentEntity>();
                Console.WriteLine($"第2次打印：{stu2.Name}");
                //解析了2次，有两个实例，stu1和stu2指向不同的两块内存，彼此之间没有关系
            }
        }

        /// <summary>
        /// 生命周期——单例
        /// </summary>
        public static void TestSingleInstance()
        {
            //注册
            ConsoleContainer.Init(x =>
                {
                    x.RegisterType<StudentEntity>().SingleInstance();
                    return x;
                }
            );
            //解析
            //直接从根域内解析（单例下可以使用）
            var stu1 = ConsoleContainer.Instance.Resolve<StudentEntity>();
            stu1.Name = "张三";
            Console.WriteLine($"第1次打印：{stu1.Name}");

            using (var scope1 = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                var stu2 = scope1.Resolve<StudentEntity>();
                Console.WriteLine($"第2次打印：{stu2.Name}");

                stu1.Name = "李四";
            }
            using (var scope2 = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                var stu3 = scope2.Resolve<StudentEntity>();
                Console.WriteLine($"第3次打印：{stu3.Name}");
            }
            //全局单例，解析3次只产生了同一个实例，指向同一个内存块
        }

        /// <summary>
        /// 生命周期域——周期域内单例
        /// </summary>
        public static void TestInstancePerLifetimeScope()
        {
            //注册
            ConsoleContainer.Init(x =>
                {
                    x.RegisterType<StudentEntity>().InstancePerLifetimeScope();
                    return x;
                }
                );
            //子域一
            using (var scope1 = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                var stu1 = scope1.Resolve<StudentEntity>();
                Console.WriteLine($"第1次打印：{stu1.Name}");

                stu1.Name = "张三";

                var stu2 = scope1.Resolve<StudentEntity>();
                Console.WriteLine($"第2次打印：{stu2.Name}");
                //解析了2次，但2次都是同一个实例（stu1和stu2指向同一个内存块Ⅰ）
            }
            //子域二
            using (var scope2 = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                var stuA = scope2.Resolve<StudentEntity>();
                Console.WriteLine($"第3次打印：{stuA.Name}");

                stuA.Name = "李四";

                var stuB = scope2.Resolve<StudentEntity>();
                Console.WriteLine($"第4次打印：{stuB.Name}");
                //解析了2次，2次都是同一内存块（stuA和stuB指向同一个内存块Ⅱ）
                //但是内存块Ⅰ与内存块Ⅱ不是同一块内存
            }
        }

        /// <summary>
        /// 生命周期域——指定周期域内单例
        /// </summary>
        public static void TestInstancePerMatchingLifetimeScope()
        {
            //注册
            ConsoleContainer.Init(x =>
                {
                    x.RegisterType<StudentEntity>().InstancePerMatchingLifetimeScope("myTag");
                    return x;
                }
            );
            //myScope标签子域一
            using (var myScope1 = ConsoleContainer.Instance.BeginLifetimeScope("myTag"))
            {
                var stu1 = myScope1.Resolve<StudentEntity>();
                stu1.Name = "张三";
                Console.WriteLine($"第1次打印：{stu1.Name}");

                var stu2 = myScope1.Resolve<StudentEntity>();
                Console.WriteLine($"第2次打印：{stu2.Name}");
                //解析了2次，但2次都是同一个实例（stu1和stu2指向同一个内存块Ⅰ）
            }
            //myScope标签子域二
            using (var myScope2 = ConsoleContainer.Instance.BeginLifetimeScope("myTag"))
            {
                var stuA = myScope2.Resolve<StudentEntity>();
                Console.WriteLine($"第3次打印：{stuA.Name}");
                //因为标签域内已注册过，所以可以解析成功
                //但是因为和上面不是同一个子域，所以解析出的实例stuA与之前的并不是同一个实例，指向另一个内存块Ⅱ
            }
            //无标签子域三
            using (var noTagScope = ConsoleContainer.Instance.BeginLifetimeScope())
            {
                try
                {
                    var stuOne = noTagScope.Resolve<StudentEntity>();//会报异常
                    Console.WriteLine($"第4次正常打印：{stuOne.Name}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"第4次异常打印：{e.Message}");
                }
                //因为StudentEntity只被注册到带有myScope标签域内，所以这里解析不到，报异常
            }
        }

        /// <summary>
        /// 生命周期域——每次请求内单例
        /// </summary>
        public static void TestInstancePerRequest()
        {
            //注册
            ConsoleContainer.Init(x =>
                {
                    x.RegisterType<StudentEntity>().InstancePerRequest();
                    //x.RegisterType<StudentEntity>().InstancePerMatchingLifetimeScope(Autofac.Core.Lifetime.MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
                    //x.RegisterType<StudentEntity>().InstancePerMatchingLifetimeScope("AutofacWebRequest");
                    return x;
                }
            );

        }
    }
}

