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
      $("#ohaha").text(data);
      console.log(data);
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
