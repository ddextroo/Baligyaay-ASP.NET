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
  function loadCategories() {
    $.ajax({
      url: "/api/Product/Category",
      method: "GET",
      success: function (data) {
        var tbody = $("#CategoryTable tbody");
        tbody.empty(); // Clear existing data
        data.forEach(function (category, index) {
          var row =
            "<tr>" +
            "<td>" +
            category.id +
            "</td>" +
            "<td>" +
            category.name +
            "</td>" +
            `<td><div class="d-flex"><i class="fa-solid fa-pencil btn text-discovery edit-category" data-id="${category.id}"></i><i class="fa-solid fa-trash btn text-danger delete-category" data-id="${category.id}"></i></div></td>` +
            "</tr>";
          tbody.append(row);
        });

        $(".delete-category")
          .off("click")
          .on("click", function () {
            var categoryId = $(this).data("id");
            deleteCategory(categoryId);
          });

        $(".edit-category")
          .off("click")
          .on("click", function () {
            var categoryId = $(this).data("id");
            editCategory(categoryId);
          });
      },
      error: function (err) {
        console.error("Error retrieving data:", err);
      },
    });
  }
  loadCategories();

  function deleteCategory(catID) {
    $.ajax({
      url: "/api/Product/Category/Delete/" + catID,
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
            loadCategories();
          }
        });
      },
    });
    loadCategories();
    setTimeout(() => {
      window.location.href = "/Home/CategoryAdmin";
    }, 500);
  }

  function editCategory(catId) {
    $.ajax({
      url: "/api/Product/GetCategoryById/" + catId,
      method: "GET",
      success: function (data) {
        $("#category_name").val(data.cat_name);

        $("#DataModal3").modal("show");

        $("#DataModal3 .modal-footer button:nth-child(2)")
          .off("click")
          .on("click", function () {
            saveCategory(catId);
          })
          .text("Save Changes");
      },
      error: function (xhr, status, error) {
        console.error("Error fetching category data:", xhr.responseText);
        Swal.fire({
          icon: "error",
          title: "Oops...",
          text: xhr.responseText,
        });
      },
    });
  }

  function saveCategory(catId) {
    var category_name = $("#category_name").val();

    $.ajax({
      url: "/api/Product/UpdateCategory/" + catId,
      type: "PATCH",
      contentType: "application/json",
      data: JSON.stringify({ id: catId, name: category_name }),
      success: function (data) {
        loadCategories();
        $("#DataModal3").modal("hide");
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
            success: function (data) {},
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
                window.location.href = "/Home/CategoryAdmin";
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
