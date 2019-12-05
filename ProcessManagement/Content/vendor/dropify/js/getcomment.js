$(function () {
    //get comments
    
    var t = {
        id:@Model.Id,
    direction: "@Direction.TaskRun"
};
getComments(t);

var form = $(".form-validation").formValidation();
var drEvent = $(".dropify").dropify();
drEvent.on('dropify.afterClear', function (event, element) {
    var element = $(this);
    element.removeAttr("data-default-file");
    element.parents(".dropify-wrapper").find(".dropify-download").remove();
    form.validate(element);
});
$(".dropify").on("change", function () {
    var element = $(this);
    element.removeAttr("data-default-file");
    element.parents(".dropify-wrapper").find(".dropify-download").remove();
    form.validate(element);
})
@if (userFile != null) {
    @: $(`<a href="@Url.Action("download", "file", new { file = userFile.Id })" class="dropify-download">Download</a>`).insertAfter($("#task-inputfile"));
}
//disable all field
disabledAllField();
//TODO: Add TaskRun Authenticate
$("#btn-savetask, #btn-donetask").on("click", function () {
    var $this = $(this);
    var actionButton = $this.attr("id");
    var isFormHaveError = form.validates();
    if (isFormHaveError) {
        showToastr("error", "Error!!");
    } else {
        var valuetext = $("#task-inputtext").val();
        var valuefile = $("#task-inputfile").val();
        var file = $("#task-inputfile")[0].files[0];
        var isEdit = $("#task-inputfile")[0].hasAttribute("data-default-file") ? true : false;
        var data = new FormData();
        data.append("idtaskrun", @Model.Id);
data.append("valuetext", valuetext);
data.append("valuefile", valuefile);
data.append("isEdit", isEdit);
data.append("fileupload", file);
if (actionButton == "btn-savetask") {
    //save task
    toggleLoading($this);
    setTimeout(function () {
        saveTaskRun(data);
        toggleLoading($this);
    }, 1000);
} else {
    //submit task
    setConfirm(`Are you sure you want to submit this task?`, `<textarea class="form-control" id="comment-action-content" placeholder="Your comment (optional)" rows="6"></textarea>`, function () {
        toggleLoading($this);
        var content = this.$content.find("#comment-action-content").val().trim();
        data.append("comment", content);
        setTimeout(function () {
            submitTaskRun(data);
            toggleLoading($this);
        }, 1000);
    });
}
                        
                }
            });
@if (Model.Status1.Name != "Open" && (idUser.ToString() != role.IdUser || isManager != true)) {
    @: $(".dropify-clear").remove();
}
$('#btn-opentask').on('click', function () {

    var $this = $(this);
    setConfirm(`Are you sure you want to open this task?`, `<textarea class="form-control" id="comment-action-content" placeholder="Your comment (optional)" rows="6"></textarea>`, function () {
        toggleLoading($this);
        var content = this.$content.find("#comment-action-content").val().trim();
        let data = {
            idtask: @Model.Id,
        comment: content
    };
    setTimeout(function () {
        submitopentask(data);
        toggleLoading($this);
    }, 1000);
});
            });;
$('#btn-finishtask').on('click', function () {
    var $this = $(this);
    let data = {
        idtask: @Model.Id
};
setConfirm(`Are you sure you want to finish this task?`, `<textarea class="form-control" id="comment-action-content" placeholder="Your comment (optional)" rows="6"></textarea>`, function () {
    toggleLoading($this);
    var content = this.$content.find("#comment-action-content").val().trim();
    let data = {
        idtask: @Model.Id,
    comment: content
};
setTimeout(function () {
    submitfinishtask(data);
    toggleLoading($this);
}, 1000);
                });
            });
$("#btn-comment-add").on("click", function () {
    var content = $("#comment-content").val();
    var $this = $(this);
    if (content.trim() != "") {
        toggleLoading($this);
        setTimeout(() => {
            var data = {
                content: content.trim(),
                id: @Model.Id,
            direction: "@Direction.TaskRun"
        };
        addComment(data);
        toggleLoading($this);
    }, 1000)
                }
            });
        });
function saveTaskRun(data) {
    $.ajax({
        url: "@Url.Action("savetaskrun", "processrun",new { area = "api"})",
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == 200) {
                showToastr("success", response.message, "toast-bottom-left")
            } else {
                showToastr("error", response.message, "toast-bottom-left")
            }
        }
    });
}
function submitTaskRun(data) {
    $.ajax({
        url: "@Url.Action("donetaskrun", "processrun",new { area = "api"})",
        type: "POST",
        data: data,
        contentType: false,
        processData: false,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == 200) {
                //showToastr("error", "Error!!", "toast-bottom-left")
                window.location.reload();
                console.error(response)
            }
        }
    });
}
function submitfinishtask(data) {
    $.ajax({
        url: "@Url.Action("submitfinishtask", "processrun",new { area = "api"})",
        type: "POST",
        data: data,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == 200) {
                //showToastr("error", "Error!!", "toast-bottom-left")
                window.location.reload();
                console.error(response)
            }
        }
    });
}

function submitopentask(data) {
    $.ajax({
        url: "@Url.Action("submitopentask", "processrun",new { area = "api"})",
        type: "POST",
        data: data,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == 200) {
                //showToastr("error", "Error!!", "toast-bottom-left")
                window.location.reload();
                console.error(response)
            }
        }
    });
}
function addComment(data) {
    $.ajax({
        url: "@Url.Action("addcomment", "processrun",new { area = "api"})",
        type: "POST",
        data: data,
        dataType: "json",
        success: function (response) {
            console.log(response);
            if (response.status == 200) {
                var t = {
                    id:@Model.Id,
                direction: "@Direction.TaskRun"
            };
            $("#comment-content").val("");
            getComments(t);
        }
    }
            });
        }
function getComments(data) {
    $.ajax({
        url: "@Url.Action("getcomments", "processrun",new { area = "api"})",
        type: "GET",
        data: data,
        dataType: "html",
        success: function (response) {
            console.log(response);
            $(".comments").hide("slow", function () {
                $(this).remove();
            });
            $(".task-comment").append(response);
        }
    });
}