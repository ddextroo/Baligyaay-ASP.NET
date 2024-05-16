$(document).ready(function () {
  $.ajax({
    url: "/api/Product/Category",
    type: "GET",
    dataType: "json",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    success: function (data) {
      $("#categorySelect").empty();
      $.each(data, function (index, category) {
        $("#categorySelect").append(
          $("<option>", {
            value: category.id,
            text: category.name,
          })
        );
      });
    },
    error: function (jqXHR, textStatus, errorThrown) {
      console.log(textStatus, errorThrown);
    },
  });

  $("#DataModal2 .modal-footer button:nth-child(2)")
    .off("click")
    .on("click", function () {
      var product = {
        name: $("#product_name").val(),
        description: $("#product_description").val(),
        price: parseFloat($("#product_price").val()),
        stock: parseInt($("#product_stock").val()),
        image: $("#product_image")[0].files.length
          ? $("#product_image")[0].files[0]
          : null,
        category: $("#categorySelect").val(),
      };
      console.log(product.image);
      var productChar = {
        material: $("#product_material").val(),
        length: parseFloat($("#product_length").val()),
        width: parseFloat($("#product_width").val()),
        height: parseFloat($("#product_height").val()),
        weight: parseFloat($("#product_weight").val()),
      };

      var requestData = {
        Product: product,
        ProductChar: productChar,
      };

      var jsonData = JSON.stringify(requestData);

      $.ajax({
        url: "/api/Product",
        type: "POST",
        contentType: "application/json",
        data: jsonData,
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        success: function (data) {
          Swal.fire({
            title: data,
            width: 600,
            padding: "3em",
            color: "#716add",
            background:
              "#fff url(https://sweetalert2.github.io/images/trees.png)",
            backdrop: `
                rgba(0,0,123,0.4)
                url("https://sweetalert2.github.io/images/nyan-cat.gif")
                left top
                no-repeat
            `,
          });
        },
        error: function (xhr, status, error) {
          console.error(xhr.responseText);
          Swal.fire({
            icon: "error",
            title: "Oops...",
            text: xhr.responseText,
          });
        },
      });
    });

  $("#DataModal3 .modal-footer button:nth-child(2)")
    .off("click")
    .on("click", function () {
      var category_name = $("#category_name").val();
      $.ajax({
        url: "/api/Product/Category",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ name: category_name }),
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        success: function (data) {
          Swal.fire({
            title: data,
            width: 600,
            padding: "3em",
            color: "#716add",
            background:
              "#fff url(https://sweetalert2.github.io/images/trees.png)",
            backdrop: `
            rgba(0,0,123,0.4)
            url("https://sweetalert2.github.io/images/nyan-cat.gif")
            left top
            no-repeat
          `,
          });
        },
        error: function (xhr, status, error) {
          console.error(xhr.responseText);
          Swal.fire({
            icon: "error",
            title: "Oops...",
            text: xhr.responseText,
          });
        },
      });
    });
});
