﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ray.EssayNotes.AutoFac.ConsoleApp.Test.TestRegister
{
    public class TestRegisterFactory
    {
        public static TestRegisterBase Create(string num)
        {
            string classFullName = $"Ray.EssayNotes.AutoFac.ConsoleApp.Test.TestRegister.TestRegister{num}";
            Type type = Type.GetType(classFullName);
            dynamic obj = type?.Assembly.CreateInstance(classFullName);
            return obj;
        }
    }
}
