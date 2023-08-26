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
    },

    CaptureVideo: function (callback) {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'video/*';  // Change this line to accept video
        //input.capture = 'camera';  // Optional: Open the camera directly
        input.onchange = function (event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function () {
		var blob = new Blob([reader.result], { type: file.type });
        	var url = URL.createObjectURL(blob);

                // The MIME type would be video, like video/mp4, video/webm etc.
                //var base64 = reader.result.replace(/^data:video\/([a-zA-Z0-9\-]+);base64,/, "");
                //var lengthBytes = lengthBytesUTF8(base64) + 1; 
                //var stringOnWasmHeap = _malloc(lengthBytes);
                //stringToUTF8(base64, stringOnWasmHeap, lengthBytes);
                //{{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);
        	var str = UnityLoader.UTF8ToString(callback);
        	var func = new Function("url", "callback", "callback(url);")(url, str);

            };
            reader.readAsDataURL(file);
        };
        input.click();
    },    
    SendScreenWidth: function (callback) {
      	var width = window.screen.width;
        var lengthBytes = lengthBytesUTF8(width.toString()) + 1; 
        var stringOnWasmHeap = _malloc(lengthBytes);
        stringToUTF8(width.toString(), stringOnWasmHeap, lengthBytes);
        
        {{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);

        //free the allocated memory.
        _free(stringOnWasmHeap);
    },

    SendScreenHeight : function (callback) {
        var height = window.innerHeight;
        var lengthBytes = lengthBytesUTF8(height.toString()) + 1; 
        var stringOnWasmHeap = _malloc(lengthBytes);
        stringToUTF8(height.toString(), stringOnWasmHeap, lengthBytes);
        
        {{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);

        //free the allocated memory.
        _free(stringOnWasmHeap);
	}
	
};

mergeInto(LibraryManager.library, MobileCamera);
