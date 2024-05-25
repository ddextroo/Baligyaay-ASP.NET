$(document).ready(function () {
  $("#logout").click(function () {
    Swal.fire({
      title: "Confirm Logout",
      text: "Please confirm that you want to log out. After logout, you will need to log in again.",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#d33",
      cancelButtonColor: "#716add",
      confirmButtonText: "Yes, Log Out",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: "/api/customer/AdminLogout",
          type: "POST",
          dataType: "json",
          success: function (data) {
            setTimeout(() => {
              window.location.href = "/Home/Admin";
            }, 500);
          },
        });
        setTimeout(() => {
          window.location.href = "/Home/Admin";
        }, 500);
      }
    });
  });
  function loadProducts() {
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
            `<td><div class="d-flex"><i class="fa-solid fa-pencil btn text-discovery edit-product" data-id="${product.prod_id}"></i><i class="fa-solid fa-trash btn text-danger delete-product" data-id="${product.prod_id}"></i></div></td>` +
            "</tr>";
          tbody.append(row);
        });

        $(".delete-product")
          .off("click")
          .on("click", function () {
            var prodId = $(this).data("id");
            deleteProduct(prodId);
          });

        $(".edit-product")
          .off("click")
          .on("click", function () {
            var prodId = $(this).data("id");
            editProduct(prodId);
          });
      },
      error: function (err) {
        console.error("Error retrieving data:", err);
      },
    });
  }

  function deleteProduct(prodId) {
    $.ajax({
      url: "/api/Product/Delete/" + prodId,
      method: "DELETE",
      dataType: "json",
      success: function (response) {
        Swal.fire({
          title: response,
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
            loadProducts();
          }
        });
      },
    });
    loadProducts();
    setTimeout(() => {
      window.location.href = "/Home/Admin";
    }, 500);
  }

  function editProduct(prodId) {
    $.ajax({
      url: "/api/Product/GetProductById/" + prodId,
      method: "GET",
      success: function (data) {
        console.log(data);
        // Fill the modal with product data
        $("#product_name").val(data.prod_name);
        $("#product_description").val(data.prod_description);
        $("#product_price").val(data.prod_price);
        $("#product_stock").val(data.prod_stock);
        $("#product_material").val(data.char_material);
        $("#product_length").val(data.char_length);
        $("#product_width").val(data.char_width);
        $("#product_height").val(data.char_height);
        $("#product_weight").val(data.char_weight);
        $("#categorySelect").val(data.cat_id);

        // Show the modal
        $("#DataModal2").modal("show");

        // Update the modal save button
        $("#DataModal2 .modal-footer button:nth-child(2)")
          .off("click")
          .on("click", function () {
            saveProduct(prodId);
          })
          .text("Save Changes");
      },
      error: function (xhr, status, error) {
        console.error("Error fetching product data:", xhr.responseText);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: xhr.responseText,
        });
      },
    });
  }

  function saveProduct(prodId) {
    var fileInput = $("#product_image")[0];
    var file = fileInput.files[0];
    var reader = new FileReader();

    reader.onloadend = function () {
      var base64Image = reader.result;

      var product = {
        name: $("#product_name").val(),
        description: $("#product_description").val(),
        price: parseFloat($("#product_price").val()),
        stock: parseInt($("#product_stock").val()),
        material: $("#product_material").val(),
        length: parseFloat($("#product_length").val()),
        width: parseFloat($("#product_width").val()),
        height: parseFloat($("#product_height").val()),
        weight: parseFloat($("#product_weight").val()),
        category: $("#categorySelect").val(),
      };

      if (file) {
        product.image = base64Image;
      } else {
        product.image = "wala";
      }

      var requestData = {
        Product: product,
        ProductChar: {
          material: $("#product_material").val(),
          length: parseFloat($("#product_length").val()),
          width: parseFloat($("#product_width").val()),
          height: parseFloat($("#product_height").val()),
          weight: parseFloat($("#product_weight").val()),
        },
      };

      $.ajax({
        url: "/api/Product/UpdateProduct/" + prodId,
        type: "PATCH",
        contentType: "application/json",
        data: JSON.stringify(requestData),
        success: function (data) {
          loadProducts();
          $("#DataModal2").modal("hide");
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

          $("#product_name").val("");
          $("#product_description").val("");
          $("#product_price").val("");
          $("#product_stock").val("");
          $("#product_material").val("");
          $("#product_length").val("");
          $("#product_width").val("");
          $("#product_height").val("");
          $("#product_weight").val("");
          $("#categorySelect").val("");
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

    if (file) {
      reader.readAsDataURL(file);
    } else {
      var product = {
        name: $("#product_name").val(),
        description: $("#product_description").val(),
        price: parseFloat($("#product_price").val()),
        stock: parseInt($("#product_stock").val()),
        material: $("#product_material").val(),
        length: parseFloat($("#product_length").val()),
        width: parseFloat($("#product_width").val()),
        height: parseFloat($("#product_height").val()),
        weight: parseFloat($("#product_weight").val()),
        category: $("#categorySelect").val(),
      };

      if (file) {
        product.image = base64Image;
      } else {
        product.image = "wala";
      }

      var requestData = {
        Product: product,
        ProductChar: {
          material: $("#product_material").val(),
          length: parseFloat($("#product_length").val()),
          width: parseFloat($("#product_width").val()),
          height: parseFloat($("#product_height").val()),
          weight: parseFloat($("#product_weight").val()),
        },
      };

      $.ajax({
        url: "/api/Product/UpdateProduct/" + prodId,
        type: "PATCH",
        contentType: "application/json",
        data: JSON.stringify(requestData),
        success: function (data) {
          loadProducts();
          $("#DataModal2").modal("hide");
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
    }
  }

  loadProducts();

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
            // imageName: fileInput.files.length ? fileInput.files[0].name : null,
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
              loadProducts();
              $("#DataModal2").modal("hide");
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
});
