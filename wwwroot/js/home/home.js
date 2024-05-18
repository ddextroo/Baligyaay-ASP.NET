$(document).ready(function () {
  $.ajax({
    url: "/api/Product",
    type: "GET",
    dataType: "json",
    headers: {
      Accept: "application/json",
      "Content-Type": "application/json",
    },
    success: function (data) {
      $.ajax({
        url: "/api/product", // Ensure this endpoint is correct and accessible
        method: "GET",
        dataType: "json",
        success: function (data) {
          console.log(data);
          var productRow = $("#product-row");
          productRow.empty(); // Clear any existing content

          data.forEach(function (product) {
            var productCard = `
                        <div class="col-6 col-md-4 mb-5 d-flex justify-content-center">
                            <div class="card" style="max-width: 350px">
                                <img
                                    src="${product.prod_img_url}"
                                    class="card-img-top"
                                    style="height: 12rem"
                                    alt="${product.prod_name} Image"
                                />
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
                                                <button class="col btn btn-outline-discovery rounded-circle">-</button>
                                                <h5 class="col fw-bold">1</h5>
                                                <button class="col btn btn-outline-discovery rounded-circle">+</button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-footer text-muted">
                                    <button class="btn btn-discovery w-100">Add to cart</button>
                                </div>
                            </div>
                        </div>
                    `;
            productRow.append(productCard);
          });
        },
        error: function (err) {
          console.error("Error fetching product data:", err);
        },
      });
    },
    error: function (jqXHR, textStatus, errorThrown) {
      console.log(textStatus, errorThrown);
    },
  });

  $('button[name="category"]:first')
    .removeClass("btn-outline-discovery")
    .addClass("btn-discovery");

  $('button[name="category"]').click(function () {
    $('button[name="category"]')
      .removeClass("btn-discovery")
      .addClass("btn-outline-discovery");

    $(this).removeClass("btn-outline-discovery").addClass("btn-discovery");
  });
});
