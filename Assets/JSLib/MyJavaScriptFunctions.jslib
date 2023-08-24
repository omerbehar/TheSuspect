var MobileCamera = {
    OpenCamera: function (callback) {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.onchange = function (event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function () {
                var base64 = reader.result.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
                var lengthBytes = lengthBytesUTF8(base64) + 1; 
                var stringOnWasmHeap = _malloc(lengthBytes);
                stringToUTF8(base64, stringOnWasmHeap, lengthBytes);
                {{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);
            };
            reader.readAsDataURL(file);
        };
        input.click();
    }
};

mergeInto(LibraryManager.library, MobileCamera);
