$(document).ready(function () {
  $.ajax({
    url: "/api/Product",
    method: "GET",
    success: function (data) {
      var tbody = $("#CustomerTable tbody");
      tbody.empty(); // Clear existing data
      data.forEach(function (product, index) {
        var row =
          "<tr>" +
          "<td>" +
          product.prod_name +
          "</td>" +
          '<td><img class="img-fluid" src="' +
          product.prod_img_url +
          '" width="100" height="100"/></td>' +
          "<td>₱" +
          product.prod_price.toFixed(2) +
          "</td>" +
          "<td>" +
          product.prod_stock +
          "</td>" +
          "<td>" +
          product.cat_name +
          "</td>" +
          "</tr>";
        tbody.append(row);
      });
    },
    error: function (err) {
      console.error("Error retrieving data:", err);
    },
  });

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
      var fileInput = $("#product_image")[0];
      var file = fileInput.files[0];

      if (file) {
        var reader = new FileReader();
        reader.onloadend = function () {
          var base64Image = reader.result;

          var product = {
            name: $("#product_name").val(),
            description: $("#product_description").val(),
            price: parseFloat($("#product_price").val()),
            stock: parseInt($("#product_stock").val()),
            image: base64Image,
            imageName: fileInput.files.length ? fileInput.files[0].name : null,
            category: $("#categorySelect").val(),
          };

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
              $.ajax({
                url: "/api/Product",
                method: "GET",
                success: function (data) {
                  var tbody = $("#CustomerTable tbody");
                  tbody.empty(); // Clear existing data
                  data.forEach(function (product, index) {
                    var row =
                      "<tr>" +
                      "<td>" +
                      product.prod_name +
                      "</td>" +
                      '<td><img class="img-fluid" src="' +
                      product.prod_img_url +
                      '" width="100" height="100"/></td>' +
                      "<td>₱" +
                      product.prod_price.toFixed(2) +
                      "</td>" +
                      "<td>" +
                      product.prod_stock +
                      "</td>" +
                      "<td>" +
                      product.cat_name +
                      "</td>" +
                      "</tr>";
                    tbody.append(row);
                  });
                },
                error: function (err) {
                  console.error("Error retrieving data:", err);
                },
              });

              $("#product_name").val("");
              $("#product_description").val("");
              $("#product_price").val("");
              $("#product_stock").val("");
              $("#product_material").val("");
              $("#product_length").val("");
              $("#product_width").val("");
              $("#product_height").val("");
              $("#product_weight").val("");
              $("#product_image").val("");
              $("#categorySelect").prop("selectedIndex", 0);

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
              }).then((result) => {
                if (result.isConfirmed) {
                  setTimeout(() => {
                    window.location.href = "/Home/Admin";
                  }, 500);
                }
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
        };

        reader.readAsDataURL(file);
      } else {
        alert("Please select an image file to upload.");
      }
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
          $("#category_name").val("");
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
          }).then((result) => {
            if (result.isConfirmed) {
              setTimeout(() => {
                window.location.href = "/Home/Admin";
              }, 500);
            }
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
