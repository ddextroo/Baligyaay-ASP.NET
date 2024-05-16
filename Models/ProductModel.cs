using System;

public class Product
{
    public int Id { get; set; }
    public required string name { get; set; }
    public required string description { get; set; }
    public required double price { get; set; }
    public required int stock { get; set; }
    public required string image { get; set; }
    public required int category { get; set; }
    public required string material { get; set; }
    public required float length { get; set; }
    public required float width { get; set; }
    public required float height { get; set; }
    public required float weight { get; set; }
}
