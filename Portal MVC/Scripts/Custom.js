
$.fn.TestFunction = function () {
    alert('Test Function');
}


$(document).ready(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip()
    });
   // alert(document.getElementById('SelectImg1').innerHTML);


    document.getElementById('SelectImg1').addEventListener('click', LoadImage);
    //UploadButton.preventDefault();
    //UploadButton.addEventListener('click', test);

    function LoadImage(e) {
        e.preventDefault();
        LaunchPicker();
    }

    function SetUploadedImages(e) {

     

        for (i = 0; i < e.filesUploaded.length; i++) {

            var ImageContainer = document.getElementById('ImgageContainer');
            var Image = document.createElement('img');
            Image.setAttribute('class', 'AttendanceImgContainer');
            Image.setAttribute('src', e.filesUploaded[i].url);
            Image.setAttribute('style', 'display:inline-block;');
            ImageContainer.appendChild(Image);

            var imageUrlBox = document.getElementById('ImageUrlsbox');
            var imageurlText2 = imageUrlBox.value;
            imageUrlBox.value = imageurlText2 + ' ' + e.filesUploaded[i].url;
        }
    }

    function ReportFileURL(e) {
        alert(e.url);
    }

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


    function LaunchPicker(e) {
        //alert('launch');
       

        const client = filestack.init('AHkEsan7gQgWv4t8ooIkQz');

        const options = {
            fromSources: ["local_file_system"],
            accept: [
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/bmp",
                "image/gif",
                "application/pdf"
            ],

            onUploadDone: file => {
                SetUploadedImages(file);
                
            }
        };
        //alert(SelectedRateID);
        client.picker(options).open();
    }
    //Removes the border from the bottom of the row above when the
    //further info row below is being displayed. 
    //$(function () {
    //    $('.FurtherDetailsBelow td').css('border-bottom', 'none');
    //})


   
});