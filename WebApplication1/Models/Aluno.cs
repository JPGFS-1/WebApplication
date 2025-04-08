using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace WebApplication1.Models
{
    public class Aluno
    {
        public string Nome { get; set; }
        public string RA { get; set; }
        public string dataNasc { get; set; }


        public static void GerarLista(HttpSessionStateBase session)
        {
            if (session["ListaAluno"] != null)
            {
                if (((List<Aluno>)session["ListaAluno"]).Count > 0)
                {
                    return;
                }
            }

            var lista = new List<Aluno>();
            lista.Add(new Aluno { Nome = "Paulo", RA = "333",dataNasc = "2020-04-08" });
            lista.Add(new Aluno { Nome = "João", RA = "222",dataNasc = "2016-04-08" });
            lista.Add(new Aluno { Nome = "Adalberto", RA = "111",dataNasc= "1998-04-08" });

            session.Remove("ListaAluno");
            session.Add("ListaAluno", lista);
        }

        public void Adicionar(HttpSessionStateBase session)
        {
            if (session["ListaAluno"] != null)
            {
                (session["ListaAluno"] as List<Aluno>).Add(this);
            }
        }
        public static Aluno Procurar(HttpSessionStateBase session, int id)
        {
            if (session["ListaAluno"] != null)
            {
                return (session["ListaAluno"] as List<Aluno>).ElementAt(id);
            }
            return null;
        }
        public void Excluir(HttpSessionStateBase session)
        {
            if (session["ListaAluno"] != null)
            {
                (session["ListaAluno"] as List<Aluno>).Remove(this);
            }
        }
        public void Editar(HttpSessionStateBase session, int id)
        {
            if (session["ListaAluno"] != null)
            {
                var aluno = Aluno.Procurar(session, id);
                aluno.Nome = this.Nome;
                aluno.RA = this.RA;
                aluno.dataNasc = this.dataNasc;
            }
        }
        
    }
}