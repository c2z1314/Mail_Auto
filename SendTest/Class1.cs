using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SendTask
{
    public class Task
    {
        private string mail_add;  //发件人的邮箱
        public string Mail_add 
        {

            get { return mail_add; }

            set { mail_add = value; }
        }

        private string mail_name; //发件人的用户名 @前面
        public string Mail_name
        {

            get { return mail_name; }

            set { mail_name = value; }
        }

        private string mail_passwd; //发件人的密码
        public string Mail_passwd
        {

            get { return mail_passwd; }

            set { mail_passwd = value; }
        }

        private List<string> mail_receive; //收件人的地址
        public List<string> Mail_receive
        {
            get { return mail_receive; }

            set { mail_receive = value; }
        }

    }
}
