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
    public class CarroController : Controller
    {
        // GET: Carro
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            Carro.GerarLista(Session);
            return View(Session["ListaCarro"] as List<Carro>);
        }

        public ActionResult Exibir(int id)
        {
            return View((Session["ListaCarro"] as List<Carro>).ElementAt(id));
        }

        public ActionResult Delete(int id)
        {
            return View((Session["ListaCarro"] as List<Carro>).ElementAt(id));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Carro carro)
        {
            Carro.Procurar(Session, id)?.Excluir(Session);

            return RedirectToAction("Listar");
        }
        public ActionResult Edit(int id)
        {
            return View(Carro.Procurar(Session, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Carro carro)
        {
            carro.Editar(Session, id);

            return RedirectToAction("Listar");
        }
        public ActionResult Create()
        {
            return View(new Carro());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Carro carro)
        {
            carro.Adicionar(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult GerarPDF()
        {
            var carros = Session["ListaCarro"] as List<Carro>; // Pega a lista de carros

            if (carros == null || carros.Count == 0)
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
                var titulo = new Paragraph("Lista de Carros\n\n", FontFactory.GetFont("Arial", 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                // Tabela
                PdfPTable table = new PdfPTable(3);
                table.WidthPercentage = 100;

                // Cabeçalho
                table.AddCell("Placa");
                table.AddCell("Ano");
                table.AddCell("Cor");

                // Linhas
                for (int i = 0; i < carros.Count; i++)
                {
                    var carro = carros[i];
                    table.AddCell(carro.Placa);
                    table.AddCell(carro.Ano.ToString());
                    table.AddCell(carro.Cor);
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray(); // Converte o conteúdo da memória em um array de bytes ( Suponho eu :P )
                return File(bytes, "application/pdf", "ListaCarros.pdf"); // Leva o PDF para o navegador, depois é só alegria :D
            }
        }
    }
}