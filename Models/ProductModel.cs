using System.ComponentModel.DataAnnotations;

public class Product
{
    public int id { get; set; }
    public required string name { get; set; }
    public required string description { get; set; }
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public double price { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Stock must be a positive integer.")]
    public int stock { get; set; }
    public string image { get; set; }
    // public string imageName { get; set; }
    public required int category { get; set; }
}
