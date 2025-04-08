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
    public class EventoController : Controller
    {
        // GET: Evento
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Listar()
        {
            Evento.GerarLista(Session);
            return View(Session["ListaEvento"] as List<Evento>);
        }

        public ActionResult Exibir(int id)
        {
            return View((Session["ListaEvento"] as List<Evento>).ElementAt(id));
        }

        public ActionResult Delete(int id)
        {
            return View((Session["ListaEvento"] as List<Evento>).ElementAt(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Evento evento)
        {
            Evento.Procurar(Session, id)?.Excluir(Session);

            return RedirectToAction("Listar");
        }
        public ActionResult Create()
        {
            return View(new Evento());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Evento evento)
        {
            evento.Adicionar(Session);

            return RedirectToAction("Listar");
        }

        public ActionResult Edit(int id)
        {
            return View(Evento.Procurar(Session, id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Evento evento)
        {
            evento.Editar(Session, id);

            return RedirectToAction("Listar");
        }
        public ActionResult GerarPDF()
        {
            var eventos = Session["ListaEvento"] as List<Evento>; // Pega a lista de eventos

            if (eventos == null || eventos.Count == 0)
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
                var titulo = new Paragraph("Lista de Eventos\n\n", FontFactory.GetFont("Arial", 16, Font.BOLD));
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                // Tabela
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;

                // Cabeçalho
                table.AddCell("Local");
                table.AddCell("Data");

                // Linhas
                for (int i = 0; i < eventos.Count; i++)
                {
                    var evento = eventos[i];
                    table.AddCell(evento.Local);
                    table.AddCell(evento.Data.ToString());
                }

                doc.Add(table);
                doc.Close();

                byte[] bytes = ms.ToArray(); // Converte o conteúdo da memória em um array de bytes ( Suponho eu :P )
                return File(bytes, "application/pdf", "ListaEventos.pdf"); // Leva o PDF para o navegador, depois é só alegria :D
            }
        }
    }
}