namespace JZXY.Duoke.server
{
    /// <summary>
    /// 定义一个用户模型
    /// </summary>
    class UserModel
    {
        public string LoginId { get; set; }

        public string LoginPwd { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
    }
}
