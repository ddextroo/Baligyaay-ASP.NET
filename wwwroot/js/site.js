$(document).ready(function () {
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
