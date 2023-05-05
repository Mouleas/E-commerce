using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Shopee.Models;

namespace Shopee.Controllers
{
    public class AdminController : Controller
    {
        // GET: AdminController
        public async Task<ActionResult> Index()
        {
            List<InventoryModel> inventoryItems = await new APICall<InventoryModel>().Get<InventoryModel>("Inventory");
            return View(inventoryItems);
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AdminController/Create
        public ActionResult CreateItem()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateItem(IFormCollection collection)
        {
            InventoryModel model = new InventoryModel();
            try
            {
                var file = Request.Form.Files.GetFile("file");
                if (file != null && file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    model.ItemName = collection["itemName"];
                    model.ItemPrice = collection["itemPrice"];
                    model.ItemQuantity = Convert.ToInt32(collection["itemQuantity"]);
                    model.ItemDescription = collection["itemDescription"];
                    model.ItemImageName = fileName;
                    model.ItemType = collection["itemType"];
                    new APICall<InventoryModel>().Post("Inventory", model);
                }
                return RedirectToAction("Index");

            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        public async Task<ActionResult> Forum()
        {
            List<ForumModel> model = await new APICall<ForumModel>().Get<ForumModel>("Forum");
            foreach (var  item in model)
            {
                item.User = (await new APICall<UserModel>().Get<UserModel>("User", item.UserId))[0];
                item.Inventory = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", item.ItemId))[0];
            }
            return View(model);
        }

        public async Task<ActionResult> ForumEdit(int id)
        {
            
            ForumModel model = (await new APICall<ForumModel>().Get<ForumModel>("Forum", id))[0];
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> ForumEdit(int id, ForumModel model)
        {

            ForumModel TmpModel = (await new APICall<ForumModel>().Get<ForumModel>("Forum", id))[0];
            model.ItemId = TmpModel.ItemId;
            model.UserId = TmpModel.UserId;
            model.ForumSubject = TmpModel.ForumSubject;
            model.ForumStatus = 1;
            try
            {
                if (model.NewMessage != null)
                {
                    
                    string newMessage = "Admin -> " + model.NewMessage + "\n"+DateTime.Now+"\n\n";
                    model.ForumBody += newMessage;
                    new APICall<ForumModel>().Put("Forum", model, id);
                }
                
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        // GET: AdminController/Delete/5
        public ActionResult Delete(int itemId)
        {
            new APICall<InventoryModel>().Delete("Inventory", itemId);
            return RedirectToAction("Index");
        }

    }
}
