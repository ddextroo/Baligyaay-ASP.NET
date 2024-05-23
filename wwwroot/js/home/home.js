$(document).ready(function () {
  $("#logout").click(function () {
    Swal.fire({
      title: "Are you sure?",
      text: "You won't be able to revert this!",
      icon: "warning",
      showCancelButton: true,
      confirmButtonColor: "#3085d6",
      cancelButtonColor: "#716add",
      confirmButtonText: "Yes, delete it!",
    }).then((result) => {
      if (result.isConfirmed) {
        $.ajax({
          url: "/api/customer/Logout",
          type: "POST",
          dataType: "json",
          success: function (data) {
            setTimeout(() => {
              window.location.href = "/Home/Login";
            }, 500);
          },
        });
      }
    });
  });
  function loadProducts(categoryId) {
    $.ajax({
      url: "/api/product",
      type: "GET",
      dataType: "json",
      data: { categoryId: categoryId },
      success: function (data) {
        var productRow = $("#product-row");
        productRow.empty();
        var productStocks = {};

        data.forEach(function (product) {
          productStocks[product.prod_id] = product.prod_stock;
          if (productStocks[product.prod_id] > 0) {
            var productCard = `
                    <div class="col-6 col-md-4 mb-5 d-flex justify-content-center">
                        <div class="card" style="max-width: 350px">
                            <img src="${product.prod_img_url}" class="card-img-top" style="height: 12rem" alt="${product.prod_name} Image"/>
                            <div class="card-body">
                                <span class="badge rounded-pill text-bg-success p-2 px-3 fw-bold">${product.cat_name}</span>
                                <h4>${product.prod_name}</h4>
                                <p class="text-body-tertiary fs-sm">${product.prod_stock} items left</p>
                                <p class="card-text">${product.prod_description}</p>
                                <div class="row d-flex align-items-center">
                                    <div class="col d-flex align-content-center justify-content-between">
                                        <p class="text-body-tertiary h-100 align-self-center">Material</p>
                                        <p class="h-100 align-self-center fs-6 fw-bold">${product.char_material}</p>
                                    </div>
                                </div>
                                <div class="row d-flex align-items-center">
                                    <div class="col d-flex align-content-center justify-content-between">
                                        <p class="text-body-tertiary h-100 align-self-center">Item Dimensions</p>
                                        <p class="h-100 align-self-center fs-6 fw-bold">${product.char_length}"L x ${product.char_width}"W x ${product.char_height}"H</p>
                                    </div>
                                </div>
                                <div class="row d-flex align-items-center">
                                    <div class="col d-flex align-content-center justify-content-between">
                                        <p class="text-body-tertiary h-100 align-self-center">Item Weight</p>
                                        <p class="h-100 align-self-center fs-6 fw-bold">${product.char_weight} Kilograms</p>
                                    </div>
                                </div>
                                <div class="row d-flex align-items-center mt-10">
                                    <div class="col d-flex align-content-center justify-content-between">
                                        <h5 class="h-100 align-self-center">₱${product.prod_price}</h5>
                                        <div class="row d-flex align-content-center">
                                            <button class="col btn btn-outline-discovery rounded-circle subtract" data-id="${product.prod_id}">-</button>
                                            <h5 class="col fw-bold quantity" data-id="${product.prod_id}">1</h5>
                                            <button class="col btn btn-outline-discovery rounded-circle add" data-id="${product.prod_id}">+</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-footer text-muted">
                                <button class="btn btn-discovery w-100 cart" data-id="${product.prod_id}" data-price="${product.prod_price}">Add to cart</button>
                            </div>
                        </div>
                    </div>
                `;
            productRow.append(productCard);
          }
        });

        productRow.off("click", ".add").on("click", ".add", function () {
          var prodId = $(this).data("id");
          var quantityElement = $('.quantity[data-id="' + prodId + '"]');
          var currentQuantity = parseInt(quantityElement.text());
          var maxStock = productStocks[prodId];

          if (currentQuantity < maxStock) {
            quantityElement.text(currentQuantity + 1);
          }
        });

        productRow
          .off("click", ".subtract")
          .on("click", ".subtract", function () {
            var prodId = $(this).data("id");
            var quantityElement = $('.quantity[data-id="' + prodId + '"]');
            var currentQuantity = parseInt(quantityElement.text());

            if (currentQuantity > 1) {
              quantityElement.text(currentQuantity - 1);
            }
          });

        productRow.off("click", ".cart").on("click", ".cart", function () {
          var prodId = $(this).data("id");
          var prod_price = $(this).data("price");
          var quantityElement = $('.quantity[data-id="' + prodId + '"]');
          var currentQuantity = parseInt(quantityElement.text());

          console.log(
            `Adding ${currentQuantity} item(s) to cart for product ID: ${prodId} | ${email}`
          );
          $.ajax({
            url: "/api/Order/checkproduct/" + cus_id,
            method: "GET",
            dataType: "json",
            data: { prod_id: prodId },
            success: function (data) {
              if (!data.productExists) {
                $.ajax({
                  type: "POST",
                  url: "/api/Order",
                  data: JSON.stringify({
                    OrderItemQuantity: currentQuantity,
                    OrderItemPrice: prod_price,
                    ProdId: prodId,
                    CusId: cus_id,
                  }),
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
                    }).then((result) => {
                      if (result.isConfirmed) {
                        window.location.href = "/Home";
                      }
                    });
                  },
                  error: function (xhr, status, error) {
                    console.log(xhr.responseText);
                    Swal.fire({
                      icon: "error",
                      title: "Oops...",
                      text: xhr.responseText,
                    });
                  },
                });
              } else {
                Swal.fire({
                  icon: "error",
                  title: "Oops...",
                  text: "The item is already exists in your cart",
                });
              }
            },
            error: function (xhr, status, error) {
              console.log(xhr.responseText);
            },
          });
        });
      },
      error: function (err) {
        console.error("Error fetching product data:", err);
      },
    });
  }

  function loadCategories() {
    $.ajax({
      url: "/api/product/category",
      type: "GET",
      dataType: "json",
      success: function (categories) {
        var categoryButtons = $("#category-buttons");
        categoryButtons.empty();

        var allButton = $("<button>", {
          type: "button",
          name: "category",
          class: "rounded-pill btn btn-outline-discovery",
          text: "All",
          "data-category-id": -1,
        });
        categoryButtons.append(allButton);

        categories.forEach(function (category) {
          var categoryButton = $("<button>", {
            type: "button",
            name: "category",
            class: "rounded-pill btn btn-outline-discovery me-1",
            text: category.name,
            "data-category-id": category.id,
          });
          categoryButtons.append(categoryButton);
        });
        loadProducts(-1);

        $('button[name="category"]:first')
          .removeClass("btn-outline-discovery")
          .addClass("btn-discovery me-1");

        // Attach click event to category buttons
        $('button[name="category"]').click(function () {
          var categoryId = $(this).data("category-id");
          $('button[name="category"]')
            .removeClass("btn-discovery")
            .addClass("btn-outline-discovery");

          $(this)
            .removeClass("btn-outline-discovery")
            .addClass("btn-discovery");

          loadProducts(categoryId);
        });
      },
      error: function (err) {
        console.error("Error fetching category data:", err);
      },
    });
  }

  loadCategories();
  var email;
  var cus_id;
  $.ajax({
    url: "/api/Customer/getcustomer",
    type: "GET",
    dataType: "json",
    data: { emailAddress: $("#email").text() },
    success: function (data) {
      email = data[0].email;
      cus_id = data[0].id;
      $.ajax({
        url: "/api/Order/getorders",
        method: "GET",
        dataType: "json",
        data: { cus_id: cus_id },
        success: function (data) {
          if (!data || Object.keys(data).length === 0) {
            $("#notif").hide();
          }
        },
        error: function (err) {
          console.error("Error retrieving data:", err);
        },
      });
    },
  });
});
