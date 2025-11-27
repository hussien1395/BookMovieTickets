using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ECommerce.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IRepository<Cart> _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(IRepository<Cart> cartRepository, IProductRepository productRepository, UserManager<ApplicationUser> userManager)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }
            var carts = await _cartRepository.GetAsync(c => c.ApplicationUserId == user.Id, [p=>p.Product]);
            return View(carts);
        }

        public async Task<IActionResult> AddToCart(int productId,int count)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var product  = await _productRepository.GetOneAsync(p => p.Id == productId);

            if (count <=0 || count> product.Quantity)
            {
                return RedirectToAction("Item" ,"Home", new {id= productId });
            }

            var existingCart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id && c.ProductId == productId);
            if (existingCart != null)
            {
                existingCart.Count += count;
                _cartRepository.Update(existingCart);
            }
            else
            {
                var cart = new Cart()
                {
                    ApplicationUserId = user.Id,
                    ProductId = productId,
                    Count = count,
                    Price = product.Price - (product.Price * (product.Discount / 100))
                };
                await _cartRepository.AddAsync(cart);
            }
          
            await _cartRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> IncrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id && c.ProductId == productId);

            if (cart != null)
            {
                if (cart.Count>=product.Quantity)
                {
                    return RedirectToAction(nameof(Index));
                }

                cart.Count += 1;
                _cartRepository.Update(cart);
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DecrementProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var product = await _productRepository.GetOneAsync(p => p.Id == productId);
            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id && c.ProductId == productId);

            if (cart != null)
            {
                if (cart.Count <=1)
                {
                    return RedirectToAction(nameof(Index));
                }

                cart.Count --;
                _cartRepository.Update(cart);
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            var cart = await _cartRepository.GetOneAsync(c => c.ApplicationUserId == user.Id && c.ProductId == productId);

            if (cart != null)
            {
                _cartRepository.Delete(cart);
                await _cartRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
