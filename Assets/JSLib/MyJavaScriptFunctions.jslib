var plugin = {
    OpenTab: function (url) {
        url = UTF8ToString(url);
        window.open(url, '_blank');
    },

    };
    mergeInto(LibraryManager.library, plugin);
var MobileCamera = 
{
    OpenCamera: function (callback, orientationCallback) {
        var input = document.createElement('input');
        input.type = 'file';
        input.accept = 'image/*';
        input.onchange = function (event) {
            console.log("LOG: input.onchange");
            var file = event.target.files[0];
// Check if a file is selected
    if (!file) {
        console.log('No file selected');
        return;
    }
    
    // Log various file properties
    console.log('File selected:', file);
    console.log('File name:', file.name);
    console.log('File type:', file.type);
    console.log('File size:', file.size); 	    // Read Exif data to get orientation
            EXIF.getData(file, function() {

                var orientation = EXIF.getTag(this, "Orientation");
                console.log("LOG: EXIF Orientation:" + orientation);

                // Pass the orientation back to Unity
                var orientationOnWasmHeap = _malloc(4); // 4 bytes for int
                setValue(orientationOnWasmHeap, orientation, 'i32');
                {{{ makeDynCall('vi', 'orientationCallback') }}}(getValue(orientationOnWasmHeap, 'i32'));
            });

            var reader = new FileReader();
            reader.onerror = function (error) {
                console.log('LOG: FileReader error:', error);
            };

            reader.onload = function () {
            
                var base64 = reader.result.replace(/^data:image\/(png|jpg|jpeg);base64,/, "");
                console.log("LOG: reader.onload: " + base64);
                var lengthBytes = lengthBytesUTF8(base64) + 1;
                console.log("LOG: lengthBytes: " + lengthBytes); 
                var stringOnWasmHeap = _malloc(lengthBytes);
                console.log("LOG: stringOnWasmHeap: " + stringOnWasmHeap);
                stringToUTF8(base64, stringOnWasmHeap, lengthBytes);
                
                {{{ makeDynCall('vi', 'callback') }}}(stringOnWasmHeap);
            };
            console.log("LOG: file: " + file.name);
            reader.readAsDataURL(file);
            input.value = '';
        };
        console.log("LOG: input.click");
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
