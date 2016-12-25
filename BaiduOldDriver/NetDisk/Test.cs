﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace NetDisk
{
    public class Test
    {
        static void CheckSuccess(Result result)
        {
            if (!result.success)
            {
                Console.WriteLine(result.exception);
                Console.ReadLine();
                Environment.Exit(-1);
            }
        }
        static void Main(string[] args)
        {
            var username = "伪红学家";
            var password = "******";
            // Test login check
            var checkResult = Authentication.LoginCheck(username);
            CheckSuccess(checkResult);
            // Handle verify code
            if (checkResult.needVCode)
            {
                File.WriteAllBytes("vcode.png", checkResult.image);
                Console.WriteLine("Verification code required. Input the text in vcode.png.");
                try
                {
                    System.Diagnostics.Process.Start("vcode.png");
                }
                catch (Exception) { }
                checkResult.verifyCode = Console.ReadLine();
                try
                {
                    File.Delete("vcode.png");
                }
                catch (Exception) { }
            }
            else Console.WriteLine("Verification code NOT required.");
            // Test login
            var loginResult = Authentication.Login(username, password, checkResult);
            CheckSuccess(loginResult);
            Console.WriteLine(loginResult.credential);
            Console.WriteLine("uid: " + loginResult.credential.uid);
            var credential = loginResult.credential;
            // Test get quota
            var quotaResult = Operation.GetQuota(credential);
            CheckSuccess(quotaResult);
            Console.WriteLine(quotaResult.used + "/" + quotaResult.total);
            // Test get user info
            var infoResult = Operation.GetUserInfo(credential);
            CheckSuccess(infoResult);
            Console.WriteLine(infoResult.records[0].uname + " " + infoResult.records[0].priority_name + " " + infoResult.records[0].avatar_url);
            // Test list file
            var fileListResult = Operation.GetFileList("/", credential);
            CheckSuccess(fileListResult);
            Console.WriteLine(string.Join("\r\n", fileListResult.list.Take(5).Select(e => e.path + " " + e.isdir + " " + e.size).ToArray()));
            // Test thumbnail
            var thumbnailResult = Operation.GetThumbnail("/1.mp4", credential);
            CheckSuccess(thumbnailResult);
            Console.WriteLine("Thumbnail " + thumbnailResult.image.Length + " bytes.");
            try
            {
                File.WriteAllBytes("thumb.jpg", thumbnailResult.image);
                var process = System.Diagnostics.Process.Start("thumb.jpg");
                process.WaitForExit();
                File.Delete("thumb.jpg");
            }
            catch (Exception) { }
            // Done
            Console.WriteLine("Success");
            Console.ReadLine();
        }
    }
}
