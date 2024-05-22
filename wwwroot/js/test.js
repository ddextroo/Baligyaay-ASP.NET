$(document).ready(function () {
  function loadTest() {
    $.ajax({
      type: "GET",
      url: "/api/Test",
      success: function (response) {
        $("#data").text("");
        response.forEach(function (item) {
          console.log(item);
          $("#data").append("<p>" + item.id + " " + item.name + "</p>");
        });
      },
      error: function (xhr, status, error) {
        console.error("Error fetching data:", status, error);
      },
    });
  }
  $("#btnsubmit").click(function () {
    var name = $("#name").val();
    var image = $("#image").val();
    $.ajax({
      type: "POST",
      url: "/api/Test",
      data: JSON.stringify({
        name: name,
        img: image,
      }),
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      success: function (response) {
        loadTest();
      },
      error: function (xhr, status, error) {
        console.error("Error fetching data:", status, error);
      },
    });
  });
  loadTest();
});
