using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AlunoController : Controller
    {
        // GET: Aluno
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            Aluno.GerarLista(Session);
            return View(Session["ListaAluno"] as List<Aluno>);
        }

        public ActionResult Exibir(int id)
        {
            return View((Session["ListaAluno"] as List<Aluno>).ElementAt(id));
        }

        public ActionResult Delete(int id)
        {
            return View((Session["ListaAluno"] as List<Aluno>).ElementAt(id));
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Aluno aluno)
        {
            Aluno.Procurar(Session, id)?.Excluir(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult Edit(int id) 
        {
            return View(Aluno.Procurar(Session, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Aluno aluno)
        {
            aluno.Editar(Session, id);

            return RedirectToAction("Listar");
        }

        public ActionResult Create()
        {
            return View(new Aluno());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Aluno aluno)
        {
            aluno.Adicionar(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult GerarPDF()
        {
            var alunos = Session["ListaAluno"] as List<Aluno>; // Pega a lista de alunos

            if (alunos == null || alunos.Count == 0)
            {
                return RedirectToAction("Listar");
            }

            // Isso ^^^ é pra garantir que tenha algo a por dentro do PDF

            using (MemoryStream ms = new MemoryStream()) // Memória temporaria para armazenar o PDF ( Eu acho :P )
            {
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30); // Tá obvio o que faz, pel'amor
                PdfWriter writer = PdfWriter.GetInstance(doc, ms); // Inicia a escrita do PDF, é enviado os dados para memória
                doc.Open(); // Vou falar nada

                // Título
                var titulo = new Paragraph("Lista de Alunos\n\n", FontFactory.GetFont("Arial", 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                // Tabela
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;

                // Cabeçalho
                table.AddCell("RA");
                table.AddCell("Nome");
                table.AddCell("Data de Nascimento");

                // Linhas
                for (int i = 0; i < alunos.Count; i++)
                {
                    var aluno = alunos[i];
                    table.AddCell(aluno.RA);
                    table.AddCell(aluno.Nome);
                    table.AddCell(aluno.DataNasc.ToString());
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray(); // Converte o conteúdo da memória em um array de bytes ( Suponho eu :P )
                return File(bytes, "application/pdf", "ListaAlunos.pdf"); // Leva o PDF para o navegador, depois é só alegria :D
            }
        }
    }
}