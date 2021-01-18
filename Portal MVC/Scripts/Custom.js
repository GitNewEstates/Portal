


$(document).ready(function () {
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
   // alert(document.getElementById('SelectImg1').innerHTML);

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


    document.getElementById('SelectImg1').addEventListener('click', LoadImage);
    document.getElementById('SelectImg2').addEventListener('click', LoadImage2);

   
    //UploadButton.preventDefault();
    //UploadButton.addEventListener('click', test);

    function LoadImage(e) {
        e.preventDefault();
        LaunchPicker(SetUploadedImages);
        //SetUploadedImages(files);
    }

    function LoadImage2(e) {
        e.preventDefault();
        LaunchPicker(AddUploadedImage);
        //SetUploadedImages(files);
    }

    function AddUploadedImage(e) {
        alert(e.filesUploaded.length);
    }

    function SetUploadedImages(e) {

     

        for (i = 0; i < e.filesUploaded.length; i++) {
           // alert('https://cdn.filestackcontent.com/AHkEsan7gQgWv4t8ooIkQz/'
             //   + 'resize=height:300,width:300/' + e.filesUploaded[i].handle);
            var ImageContainer = document.getElementById('ImgageContainer');
            var Image = document.createElement('img');
            Image.setAttribute('class', 'AttendanceImgContainer');
            Image.setAttribute('src', 'https://cdn.filestackcontent.com/AHkEsan7gQgWv4t8ooIkQz/'
               + 'resize=height:300,width:300/' + e.filesUploaded[i].handle);
            Image.setAttribute('style', 'display:inline-block;');
            ImageContainer.appendChild(Image);

            var imageUrlBox = document.getElementById('ImageUrlsbox');
            var imageurlText2 = imageUrlBox.value;
            imageUrlBox.value = imageurlText2 + ' ' + e.filesUploaded[i].handle;
        }
    }

    function FileSelected(e) {
        //alert();
    }
    
    function LaunchPicker(CallBackFunction) {

        const client = filestack.init('AHkEsan7gQgWv4t8ooIkQz');

        const options = {
            fromSources: ["local_file_system", "video"],
            accept: [
                "image/jpeg",
                "image/jpg",
                "image/png",
                "image/bmp",
                "image/gif",
                "application/pdf",
                "video/*"
            ],

            onUploadDone: file => {
                CallBackFunction(file);
                //SetUploadedImages(file);
                
            },

            onFileSelected: file => {
                //alert(file.size);
                //FileSelected(file);
            }
        };
        client.picker(options).open();
    }

   
});