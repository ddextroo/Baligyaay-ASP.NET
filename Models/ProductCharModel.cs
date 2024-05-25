using System;
using System.ComponentModel.DataAnnotations;

public class ProductChar
{
    public int id { get; set; }
    public required string material { get; set; }
    [Range(0.1, double.MaxValue, ErrorMessage = "Length must be a positive value.")]
    public float length { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Width must be a positive value.")]
    public float width { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Height must be a positive value.")]
    public float height { get; set; }

    [Range(0.1, double.MaxValue, ErrorMessage = "Weight must be a positive value.")]
    public float weight { get; set; }
}
