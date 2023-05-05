using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Shopee.Models;
using System.Runtime.CompilerServices;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Shopee.Controllers
{
    public class UserController : Controller
    {

        IConfiguration _configuration;
        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // GET: UserController
        public async Task<ActionResult> Index(int userId)
        {
            ViewBag.UserId = userId;
            List<InventoryModel> model = await new APICall<InventoryModel>().Get<InventoryModel>("Inventory");
            return View(model);
        }

        public ActionResult Search(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }
        //public ActionResult Editprofile(UserModel user)
        //{
        //    return View(user);
        //}

        public async Task<ActionResult> UpdateProfile(int userId)
        {
            Console.WriteLine("In edit profile");
            UserModel model = (await new APICall<UserModel>().Get<UserModel>("User", userId))[0];
            return View(model);
        }

        // GET: UserController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public async Task<ActionResult> ItemDetails(int userId, int itemId)
        {
            ViewBag.UserId = userId;
            InventoryModel model = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", itemId))[0];
            return View(model);
        }

        // GET: UserController/Create
        public ActionResult CreateForum(int itemId, int userId)
        {
            ViewBag.ItemId = itemId;
            ViewBag.UserId = userId;
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateForum(int itemId, int userId, IFormCollection collection)
        {
            UserModel User = (await new APICall<UserModel>().Get<UserModel>("User", userId))[0];
            ForumModel model = new ForumModel();
            model.ForumSubject = "" + collection["forumSubject"];
            model.ForumBody = User.UserName +" -> " + collection["forumBody"] + "\n" + DateTime.Now + "\n\n";
            model.ForumStatus = 0;
            model.ItemId = itemId;
            model.UserId = userId;
            new APICall<ForumModel>().Post("Forum", model);
            await Task.Delay(2000);
            ViewBag.UserId = userId;
            return RedirectToAction("ViewForum", new {userId = userId});
        }

        public async Task<ActionResult> ViewForum(int userId)
        {
            List<ForumModel> AllForums = await new APICall<ForumModel>().Get<ForumModel>("Forum");
            List<ForumModel> model = new List<ForumModel>();
            foreach (var item in AllForums)
            {
                if (item.UserId == userId)
                {
                    model.Add(item);
                }
            }
            ViewBag.UserId = userId;
            return View(model);
        }
        public async Task<ActionResult> ForumEdit(int forumId, int userId)
        {
            ForumModel model = (await new APICall<ForumModel>().Get<ForumModel>("Forum", forumId))[0];
            ViewBag.UserId = userId;
            return View(model);
        }
        public async Task<ActionResult> DeleteForum(int forumId, int userId)
        {
            new APICall<ForumModel>().Delete("Forum", forumId);
            await Task.Delay(2000);
            return RedirectToAction("ViewForum", new { userId = userId});
        }

        [HttpPost]
        public async Task<ActionResult> ForumUpdate(int forumId, int userId, ForumModel model)
        {
            if (model.NewMessage != null)
            {
                UserModel User = (await new APICall<UserModel>().Get<UserModel>("User", userId))[0];
                string newMessage = User.UserName + " -> " + model.NewMessage + "\n" + DateTime.Now + "\n\n";
                model.ForumBody += newMessage;
                new APICall<ForumModel>().Put("Forum", model, forumId);
            }
            
            return RedirectToAction("ViewForum", new {userId = userId});
        }

        public async void AddOrUpdateCart(int itemid, int userid)
        {
            try
            {
                SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShopeeContext"));
                connection.Open();
                SqlCommand command = new SqlCommand("GetCart", connection);
                command.Parameters.AddWithValue("@userid", userid);
                command.Parameters.AddWithValue("@inventoryid", itemid);
                command.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader reader = command.ExecuteReader();
                InventoryModel model = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", itemid))[0];
                if (model.ItemQuantity-1 >= 0)
                {
                    model.ItemQuantity--;
                    new APICall<InventoryModel>().Put("Inventory", model, itemid);
                    bool found = false;
                    int quantity = -1;
                    while (reader.Read())
                    {
                        found = true;
                        quantity = Convert.ToInt32(reader["Quantity"]);
                    }
                    reader.Close();
                    if (found)
                    {
                        quantity++;
                        command = new SqlCommand($"UPDATE CartModel SET Quantity={quantity} WHERE UserId={userid} and InventoryId={itemid}", connection);
                        command.ExecuteNonQuery();
                    }
                    else
                    {
                        command = new SqlCommand($"INSERT INTO CartModel Values({userid},{itemid},1)", connection);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task<ActionResult> AddOrSubQuantity(int userId, int cartId, int ops)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ShopeeContext"));
            connection.Open();
            CartModel model = (await new APICall<CartModel>().Get<CartModel>("Cart", cartId))[0];
            model.User = (await new APICall<UserModel>().Get<UserModel>("User", model.UserId))[0];
            InventoryModel inventory = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", model.InventoryId))[0];
            if (ops == 1)
            {
                if (inventory.ItemQuantity-1 >= 0)
                {
                    inventory.ItemQuantity--;
                    int quantity = model.Quantity+1;
                    SqlCommand command = new SqlCommand($"UPDATE CartModel SET Quantity={quantity} WHERE UserId={model.UserId} and InventoryId={model.InventoryId}", connection);
                    new APICall<InventoryModel>().Put("Inventory", inventory, model.InventoryId);
                    command.ExecuteNonQuery();
                }
            }
            else
            {
                if (model.Quantity-1 <= 0)
                {
                    new APICall<CartModel>().Delete("Cart", model.CartId);
                    inventory.ItemQuantity++;
                    new APICall<InventoryModel>().Put("Inventory", inventory, model.InventoryId);
                    await Task.Delay(2000);
                }
                else
                {
                    inventory.ItemQuantity++;
                    new APICall<InventoryModel>().Put("Inventory", inventory, model.InventoryId);
                    int quantity = model.Quantity - 1;
                    SqlCommand command = new SqlCommand($"UPDATE CartModel SET Quantity={quantity} WHERE UserId={model.UserId} and InventoryId={model.InventoryId}", connection);
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Cart", new
            {
                userId = userId
            });
        }
        public async Task<ActionResult> AddCart(int itemId, int userId)
        {
            AddOrUpdateCart(itemId, userId);
            await Task.Delay(2000);
            return RedirectToAction("Index", new {userId=userId});
        }
        public async Task<ActionResult> Cart(int userId)
        {
            List<CartModel> AllCarts = await new APICall<CartModel>().Get<CartModel>("Cart");
            List<CartModel> model = new List<CartModel>();
            int total = 0;
            foreach (var item in AllCarts)
            {
                if (item.UserId == userId)
                {
                    item.User = (await new APICall<UserModel>().Get<UserModel>("User", item.UserId))[0];
                    item.Inventory = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", item.InventoryId))[0];
                    total += Convert.ToInt32(item.Inventory.ItemPrice)*item.Quantity;
                    model.Add(item);
                }
            }
            ViewBag.total = total;
            ViewBag.UserId = userId;
            return View(model);
        }


        public async Task<ActionResult> Buy(int userId)
        {
            List<CartModel> AllCarts = await new APICall<CartModel>().Get<CartModel>("Cart");
            foreach (var item in AllCarts)
            {
                
                if (item.UserId == userId)
                {
                    item.User = (await new APICall<UserModel>().Get<UserModel>("User", item.UserId))[0];
                    item.Inventory = (await new APICall<InventoryModel>().Get<InventoryModel>("Inventory", item.InventoryId))[0];
                    OrderModel order = new OrderModel()
                    {
                        UserId = userId,
                        ItemName = item.Inventory.ItemName,
                        ItemType = item.Inventory.ItemType,
                        Quantity = item.Quantity,
                        TotalAmount = item.Quantity * Convert.ToInt32(item.Inventory.ItemPrice),
                        Date = DateTime.Now
                    };
                    new APICall<OrderModel>().Post("Order", order);
                    new SendEmail().SendOrderDetails(order, item);
                    new APICall<CartModel>().Delete("Cart", item.CartId);
                }
            }
            return RedirectToAction("Index", new {userId = userId});
        }

        public async Task<ActionResult> Order(int userId)
        {
            List<OrderModel> AllOrders = await new APICall<OrderModel>().Get<OrderModel>("Order");
            List<OrderModel> model = new List<OrderModel>();
            foreach (var items in AllOrders)
            {
                if (items.UserId == userId)
                {
                    model.Add(items);
                }
            }
            model.Reverse();
            ViewBag.UserId = userId;
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Search(int userId, IFormCollection collection)
        {
            string name = collection["itemname"];
            string tmpPrice = collection["price"];
            string category = collection["category"];
            int price = 0;
            if (tmpPrice != "")
            {
                price = Convert.ToInt32(tmpPrice);
            }
            ViewBag.UserId = userId;
            List<InventoryModel> AllItems = await new APICall<InventoryModel>().Get<InventoryModel>("Inventory");
            List<InventoryModel> model = new List<InventoryModel>();
            foreach (var item in AllItems)
            {
                if (Convert.ToInt32(item.ItemPrice) >= price && item.ItemName.Contains(name, StringComparison.OrdinalIgnoreCase) && item.ItemType.Contains(category, StringComparison.OrdinalIgnoreCase))
                {
                    model.Add((InventoryModel)item);
                } 
            }
            return View(model);
        }


        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateProfile(int userId, UserModel user)
        {
            UserModel tmpUser = (await new APICall<UserModel>().Get<UserModel>("User", userId))[0];
            user.UserEmail = tmpUser.UserEmail;

            new APICall<UserModel>().Put("User", user, userId);
            return RedirectToAction("Index", new { userId = userId });
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int forumId, int userId, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index), new {userId = userId});
            }
            catch
            {
                return View();
            }
        }
    }
}
