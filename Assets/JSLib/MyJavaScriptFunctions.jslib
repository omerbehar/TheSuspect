var MobileCamera = 
{
	getOrientation: function(callback) 
	{
        var input = document.createElement('input');
        var orientationOnWasmHeap = _malloc(4); // 4 bytes for int
        input.type = 'file';
        input.accept = 'image/*';
        input.onchange = function (event) 
	    {    
	        var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function(e) 
            {
                var view = new DataView(e.target.result);
                if (view.getUint16(0, false) != 0xFFD8)
                {
                    console.log("Invalid JPEG");
                    setValue(orientationOnWasmHeap, -2, 'i32');
                    {{{ makeDynCall('vi', 'callback') }}}(getValue(orientationOnWasmHeap, 'i32'));
                }
                var length = view.byteLength, offset = 2;
                while (offset < length) 
                {
                    if (view.getUint16(offset+2, false) <= 8) return callback(-1);
                    var marker = view.getUint16(offset, false);
                    offset += 2;
                    if (marker == 0xFFE1) 
                    {
                        if (view.getUint32(offset += 2, false) != 0x45786966) 
                        {
                            //debug
                            console.log("Invalid Exif data");
                            setValue(orientationOnWasmHeap, -1, 'i32');
                            {{{ makeDynCall('vi', 'callback') }}}(getValue(orientationOnWasmHeap, 'i32'));
                        }
                        var little = view.getUint16(offset += 6, false) == 0x4949;
                        offset += view.getUint32(offset + 4, little);
                        var tags = view.getUint16(offset, little);
                        offset += 2;
                        for (var i = 0; i < tags; i++)
                        {
                            if (view.getUint16(offset + (i * 12), little) == 0x0112)
                            {
                                console.log("Orientation: " + view.getUint16(offset + (i * 12) + 8, little));
                                setValue(orientationOnWasmHeap, view.getUint16(offset + (i * 12) + 8, 'i32'));
                                {{{ makeDynCall('vi', 'callback') }}}(getValue(orientationOnWasmHeap, 'i32'));
                            }
                        }
                    }
                    else if ((marker & 0xFF00) != 0xFF00)
                    {
                        break;
                    }
                    else
                    { 
                        offset += view.getUint16(offset, false);
                    }
                }
                setValue(orientationOnWasmHeap, -1, 'i32');
                {{{ makeDynCall('vi', 'callback') }}}(getValue(orientationOnWasmHeap, 'i32'));
            };
        reader.readAsArrayBuffer(file);
        };
        input.click();
    },

    OpenCamera: function (callback, orientationCallback) {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.onchange = function (event) {
            var file = event.target.files[0];
 	    // Read Exif data to get orientation
            EXIF.getData(file, function() {
		console.log(EXIF.getAllTags(this));

                var orientation = EXIF.getTag(this, "Orientation");
                console.log("EXIF Orientation:", orientation);

                // Pass the orientation back to Unity
                var orientationOnWasmHeap = _malloc(4); // 4 bytes for int
                setValue(orientationOnWasmHeap, orientation, 'i32');
                {{{ makeDynCall('vi', 'orientationCallback') }}}(getValue(orientationOnWasmHeap, 'i32'));
            });

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

    CaptureVideo: function (callback, HandleError) {
    //try
    //{
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'video/*';  // Change this line to accept video
        //input.capture = 'camera';  // Optional: Open the camera directly
	input.onchange = function (event) {
            var file = event.target.files[0];
            var reader = new FileReader();
            reader.onload = function () {
                
                // The MIME type would be video, like video/mp4, video/webm etc.
                var base64 = reader.result.replace(/^data:video\/([a-zA-Z0-9\-]+);base64,/, "");
                var lengthBytes = lengthBytesUTF8(base64) + 1; 
                var stringOnWasmHeap = _malloc(lengthBytes);
                stringToUTF8(base64, stringOnWasmHeap, lengthBytes);
                {{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);
            _free(stringOnWasmHeap);
		};
            reader.readAsDataURL(file);        
        };
        input.click();
        //}
        //catch (error) {
            //console.error("An error occurred: " + error.message);
        
            // Convert the error message to a pointer on the WebAssembly heap
            //var lengthBytes = lengthBytesUTF8(error.message) + 1;
            //var stringOnWasmHeap = _malloc(lengthBytes);
            //stringToUTF8(error.message, stringOnWasmHeap, lengthBytes);
        
            // Send the pointer to the error-handling callback in Unity
            //{{{ makeDynCall('vi', 'HandleError') }}}(stringOnWasmHeap);
            //_free(stringOnWasmHeap);
        //}
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
