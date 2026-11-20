
const int MaxProducts = 100;
const int MaxSales = 500;
const int LowStockLimit = 5;
const decimal TaxRate = 5m;

// Product arrays
int[] productIds = new int[MaxProducts];
string[] productNames = new string[MaxProducts];
decimal[] productPrices = new decimal[MaxProducts];
int[] productStocks = new int[MaxProducts];
int productCount = 4;

// Dummy data
int[] ids = { 101, 102, 103, 104 };
string[] names = { "Pen", "Pencil", "Notebook", "Box" };
decimal[] prices = { 10m, 5m, 50m, 100m };
int[] stocks = { 20, 30, 4, 3 };

Array.Copy(ids, productIds, productCount);
Array.Copy(names, productNames, productCount);
Array.Copy(prices, productPrices, productCount);
Array.Copy(stocks, productStocks, productCount);

// Sales arrays - one record per completed bill
int[] saleBillNumbers = new int[MaxSales];
string[] saleCustomers = new string[MaxSales];
decimal[] saleAmounts = new decimal[MaxSales];
int saleCount = 0;

// Main menu
while (true)
{
    Console.WriteLine("\n===== RETAIL STORE MANAGEMENT =====");
    Console.WriteLine("1. Add Product");
    Console.WriteLine("2. Inventory Report");
    Console.WriteLine("3. Search Product");
    Console.WriteLine("4. Customer Purchase & Billing");
    Console.WriteLine("5. Low-Stock Products");
    Console.WriteLine("6. Sales History");
    Console.WriteLine("0. Exit");

    Console.Write("\nEnter choice: ");
    string choice = Console.ReadLine() ?? "";

    switch (choice)
    {
        case "1":
            AddProduct();
            break;

        case "2":
            InventoryReport();
            break;

        case "3":
            SearchProduct();
            break;

        case "4":
            GenerateBill();
            break;

        case "5":
            LowStockReport();
            break;

        case "6":
            SalesHistory();
            break;

        case "0":
            Console.WriteLine("Thank you!");
            return;

        default:
            Console.WriteLine("Invalid choice.");
            break;
    }
}

// Add product
void AddProduct()
{
    if (productCount == MaxProducts)
    {
        Console.WriteLine("Product storage is full.");
        return;
    }

    Console.Write("\nProduct ID: ");
    int id = int.Parse(Console.ReadLine()!);

    if (Array.IndexOf(productIds, id, 0, productCount) != -1)
    {
        Console.WriteLine("Product ID already exists.");
        return;
    }

    Console.Write("Product Name: ");
    string name = Console.ReadLine() ?? "";

    Console.Write("Unit Price: ");
    decimal price = decimal.Parse(Console.ReadLine()!);

    Console.Write("Available Quantity: ");
    int stock = int.Parse(Console.ReadLine()!);

    if (price <= 0 || stock < 0)
    {
        Console.WriteLine("Invalid price or quantity.");
        return;
    }

    productIds[productCount] = id;
    productNames[productCount] = name;
    productPrices[productCount] = price;
    productStocks[productCount] = stock;

    productCount++;

    Console.WriteLine("Product added successfully!");
}

// Display products
void DisplayProducts()
{
    Console.WriteLine("\n--- PRODUCT LIST ---");
    Console.WriteLine("ID\tProduct\tPrice\tStock");

    for (int i = 0; i < productCount; i++)
    {
        Console.WriteLine(
            $"{productIds[i]}\t{productNames[i]}\t" +
            $"{productPrices[i]:N2}\t{productStocks[i]}");
    }
}

// Search product
void SearchProduct()
{
    Console.Write("\nEnter Product ID: ");
    int id = int.Parse(Console.ReadLine()!);

    int index = Array.IndexOf(productIds, id, 0, productCount);

    if (index == -1)
    {
        Console.WriteLine("Product not found.");
        return;
    }

    Console.WriteLine($"Product: {productNames[index]}");
    Console.WriteLine($"Price: {productPrices[index]:N2}");
    Console.WriteLine($"Stock: {productStocks[index]}");
}

// Calculate discount
decimal CalculateDiscount(decimal subtotal, decimal percentage)
{
    return subtotal * percentage / 100;
}

// Calculate tax
decimal CalculateTax(decimal amount)
{
    return amount * TaxRate / 100;
}

// Customer purchase
void GenerateBill()
{
    if (productCount == 0)
    {
        Console.WriteLine("No products available.");
        return;
    }

    if (saleCount == MaxSales)
    {
        Console.WriteLine("Sales storage is full.");
        return;
    }

    Console.Write("\nCustomer Name: ");
    string customer = Console.ReadLine() ?? "";

    // Cart arrays
    int[] cartIndexes = new int[MaxProducts];
    int[] cartQuantities = new int[MaxProducts];
    int cartCount = 0;

    while (true)
    {
        Console.WriteLine("\n--- PURCHASE MENU ---");
        Console.WriteLine("1. Add Product to Cart");
        Console.WriteLine("2. View Cart");
        Console.WriteLine("3. Checkout");
        Console.WriteLine("4. Cancel Purchase");

        Console.Write("Enter choice: ");
        string choice = Console.ReadLine() ?? "";


        if (choice == "1")
        {
            string answer = "Y";

            while (answer.ToUpper() == "Y")
            {
                DisplayProducts();

                Console.Write("\nEnter Product ID: ");
                int id = int.Parse(Console.ReadLine()!);

                int index = Array.IndexOf(productIds, id, 0, productCount);

                if (index == -1)
                {
                    Console.WriteLine("Product not found.");
                    continue;
                }

                Console.Write("Enter Quantity: ");
                int quantity = int.Parse(Console.ReadLine()!);

                int cartIndex = Array.IndexOf(cartIndexes, index, 0, cartCount);

                int alreadyAdded = cartIndex == -1 ? 0 : cartQuantities[cartIndex];

                if (quantity <= 0 || quantity + alreadyAdded > productStocks[index])
                {
                    Console.WriteLine("Invalid quantity or insufficient stock.");
                    continue;
                }

                if (cartIndex == -1)
                {
                    cartIndexes[cartCount] = index;
                    cartQuantities[cartCount] = quantity;
                    cartCount++;
                }
                else
                {
                    cartQuantities[cartIndex] += quantity;
                }

                Console.WriteLine("Product added to cart!");

                Console.Write("Purchase more? (Y/N): ");
                answer = Console.ReadLine() ?? "N";
            }
        }
        else if (choice == "2")
        {
            ViewCart(cartIndexes, cartQuantities, cartCount);
        }
        else if (choice == "3")
        {
            if (cartCount == 0)
            {
                Console.WriteLine("Cart is empty.");
                continue;
            }

            decimal subtotal = CalculateTotal(cartIndexes, cartQuantities, cartCount);

            Console.Write("Enter Discount Percentage: ");
            decimal discountPercent = decimal.Parse(Console.ReadLine()!);

            if (discountPercent < 0 || discountPercent > 100)
            {
                Console.WriteLine("Invalid discount.");
                continue;
            }

            decimal discount = CalculateDiscount(subtotal, discountPercent);

            decimal taxableAmount = subtotal - discount;
            decimal tax = CalculateTax(taxableAmount);
            decimal grandTotal = taxableAmount + tax;

            int billNumber = 1001 + saleCount;

            Console.WriteLine("\n========== CUSTOMER BILL ==========");
            Console.WriteLine($"Bill No : {billNumber}");
            Console.WriteLine($"Customer: {customer}");
            Console.WriteLine($"Date    : {DateTime.Now:dd-MM-yyyy}");

            ViewCart(cartIndexes, cartQuantities, cartCount);

            Console.WriteLine($"Subtotal      : Rs. {subtotal:N2}");
            Console.WriteLine($"Discount      : Rs. {discount:N2}");
            Console.WriteLine($"Tax (5%)      : Rs. {tax:N2}");
            Console.WriteLine($"Grand Total   : Rs. {grandTotal:N2}");
            Console.WriteLine("===================================");
            Console.WriteLine("Thank you for shopping!");

            // Update inventory
            for (int i = 0; i < cartCount; i++)
            {
                int index = cartIndexes[i];
                productStocks[index] -= cartQuantities[i];
            }

            // Save sales record
            saleBillNumbers[saleCount] = billNumber;
            saleCustomers[saleCount] = customer;
            saleAmounts[saleCount] = grandTotal;
            saleCount++;

            Console.WriteLine("Purchase completed!");
            return;
        }
        else if (choice == "4")
        {
            Console.WriteLine("Purchase cancelled.");
            return;
        }
        else
        {
            Console.WriteLine("Invalid choice.");
        }
    }
}

// View cart
void ViewCart(int[] indexes, int[] quantities, int count)
{
    Console.WriteLine("\n--- YOUR CART ---");
    Console.WriteLine("Product\tQty\tPrice\tAmount");

    for (int i = 0; i < count; i++)
    {
        int index = indexes[i];

        decimal amount = productPrices[index] * quantities[i];

        Console.WriteLine(
            $"{productNames[index]}\t{quantities[i]}\t" +
            $"{productPrices[index]:N2}\t{amount:N2}");
    }

    Console.WriteLine(
        $"Subtotal: Rs. {CalculateTotal(indexes, quantities, count):N2}");
}

// Calculate cart total
decimal CalculateTotal(int[] indexes, int[] quantities, int count)
{
    decimal total = 0;

    for (int i = 0; i < count; i++)
    {
        total += productPrices[indexes[i]] * quantities[i];
    }

    return total;
}

// Low-stock report
void LowStockReport()
{
    Console.WriteLine("\n--- LOW-STOCK PRODUCTS ---");

    bool found = false;

    for (int i = 0; i < productCount; i++)
    {
        if (productStocks[i] <= LowStockLimit)
        {
            Console.WriteLine($"{productNames[i]} - Stock: {productStocks[i]}");
            found = true;
        }
    }

    if (!found)
        Console.WriteLine("No low-stock products.");
}

// Inventory report
void InventoryReport()
{
    DisplayProducts();

    int totalUnits = 0;
    decimal inventoryValue = 0;

    for (int i = 0; i < productCount; i++)
    {
        totalUnits += productStocks[i];
        inventoryValue += productPrices[i] * productStocks[i];
    }

    Console.WriteLine($"\nTotal Products: {productCount}");
    Console.WriteLine($"Total Units: {totalUnits}");
    Console.WriteLine($"Inventory Value: Rs. {inventoryValue:N2}");
}

// Sales history
void SalesHistory()
{
    Console.WriteLine("\n--- SALES HISTORY ---");

    if (saleCount == 0)
    {
        Console.WriteLine("No sales recorded.");
        return;
    }

    decimal totalRevenue = 0;

    for (int i = 0; i < saleCount; i++)
    {
        Console.WriteLine(
            $"Bill: {saleBillNumbers[i]} | " +
            $"Customer: {saleCustomers[i]} | " +
            $"Amount: {saleAmounts[i]:N2}");

        totalRevenue += saleAmounts[i];
    }

    Console.WriteLine($"\nTotal Bills: {saleCount}");
    Console.WriteLine($"Total Revenue: Rs. {totalRevenue:N2}");
}
