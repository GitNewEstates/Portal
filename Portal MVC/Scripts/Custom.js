
$.fn.TestFunction = function () {
    alert('Test Function');
}


$(document).ready(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });

    //Applies to table rows that are initially hidded but can be shown to 
    //display further info

    $('.ShowDetails').click(function (e) {
        var tr = $(this).closest('tr');
       
        //if has show class then hide
        if (tr.next('tr').hasClass('HideTableRow')) {
            $(tr).next('tr').removeClass('HideTableRow').addClass('ShowTableRow');
            $tr.addClass("ShowTableRow");
   
        } else {

            $(tr).next(tr).removeClass('ShowTableRow').addClass('HideTableRow');
            $(tr).addClass("ShowTableRow");
        }
    });

    $('.CollapseTableRow').click(function (e) {
        
        var tr = $(this).closest('tr');

        //$(tr).fadeOut(1000)

        //setTimeout(function () {
        //    $(tr).removeClass('ShowTableRow').addClass('HideTableRow');
        //    $(tr).closest('tr').removeClass('ShowTableRow');
        //}, 500);
        
            $(tr).removeClass('ShowTableRow').addClass('HideTableRow');
    });


    //Removes the border from the bottom of the row above when the
    //further info row below is being displayed. 
    //$(function () {
    //    $('.FurtherDetailsBelow td').css('border-bottom', 'none');
    //})
});