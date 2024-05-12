$(document).ready(function () {
  $.ajax({
    type: "GET",
    url: "/api/Customer",
    dataType: "json",
    success: function (data) {
      console.log(data);
    },
  });
});
