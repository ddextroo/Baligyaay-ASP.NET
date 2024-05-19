public class CustomerAfterOrder
{
    // OrderItem properties
    public int OrderItemId { get; set; }
    public int OrderItemQuantity { get; set; }
    public decimal OrderItemPrice { get; set; }

    // Customer properties
    public int CusId { get; set; }
    public string CusFname { get; set; }
    public string CusLname { get; set; }
    public string CusEmail { get; set; }
    public string CusPhone { get; set; }

    // Product properties
    public int ProdId { get; set; }
    public string ProdName { get; set; }
    public string ProdDescription { get; set; }
    public decimal ProdPrice { get; set; }
    public int ProdStock { get; set; }
    public string ProdImgUrl { get; set; }

    // Category properties
    public int CatId { get; set; }
    public string CatName { get; set; }

    // ProductChar properties
    public int CharId { get; set; }
    public string CharMaterial { get; set; }
    public decimal CharLength { get; set; }
    public decimal CharWidth { get; set; }
    public decimal CharHeight { get; set; }
    public decimal CharWeight { get; set; }
}
