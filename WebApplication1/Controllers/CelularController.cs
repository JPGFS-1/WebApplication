using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text.pdf;
using iTextSharp.text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CelularController : Controller
    {
        // GET: Celular
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Listar()
        {
            Celular.GerarLista(Session);
            return View(Session["ListaCelular"] as List<Celular>);
        }

        public ActionResult Exibir(int id)
        {
            return View((Session["ListaCelular"] as List<Celular>).ElementAt(id));
        }

        public ActionResult Delete(int id)
        {
            return View((Session["ListaCelular"] as List<Celular>).ElementAt(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Celular celular)
        {
            Celular.Procurar(Session, id)?.Excluir(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult Edit(int id)
        {
            return View(Celular.Procurar(Session, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Celular celular)
        {
            celular.Editar(Session, id);

            return RedirectToAction("Listar");
        }

        public ActionResult Create()
        {
            return View(new Celular());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Celular celular)
        {
            celular.Adicionar(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult GerarPDF()
        {
            var celulares = Session["ListaCelular"] as List<Celular>; // Pega a lista de celulares

            if (celulares == null || celulares.Count == 0)
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
                var titulo = new Paragraph("Lista de Celulares\n\n", FontFactory.GetFont("Arial", 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                // Tabela
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;

                // Cabeçalho
                table.AddCell("Numero");
                table.AddCell("Marca");
                table.AddCell("Novo");

                // Linhas
                for (int i = 0; i < celulares.Count; i++)
                {
                    var celular = celulares[i];
                    table.AddCell(celular.Numero.ToString());
                    table.AddCell(celular.Marca);
                    table.AddCell(celular.Novo.ToString());
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray(); // Converte o conteúdo da memória em um array de bytes ( Suponho eu :P )
                return File(bytes, "application/pdf", "ListaCelulares.pdf"); // Leva o PDF para o navegador, depois é só alegria :D
            }
        }
    }
}