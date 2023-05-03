﻿namespace API_Parceiros_Da_Rua.Model
{
    public class UsersViewModel
    {
        // idusers, nome, email, gestao, datacadastro FROM users
        public int idUsers { get; set; }
        public string nome { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string gestao { get; set; } = string.Empty;

        public DateTime dataCadastro { get; set; } = DateTime.Now;
    }
}