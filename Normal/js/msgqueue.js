function Queue(serializer = null) {
    this.data = [];
    this.enqueue = function() {
        //this.data.push(ele);
        var eles = arguments;
        for (var i = 0; i < eles.length; ++i) {
            this.data.push(eles[i]);
        }
    };
    this.dequeue = function() {
        return this.data.shift();
    };
    this.front = function() {
        return this.data[0];
    };
    this.back = function() {
        return this.data[this.data.length - 1];
    };
    this.dequeuebatch = function(condition) { // condition = obj => obj.xxx == 'search';
        var reslist = [];
        for (var i = 0; i < this.data.length; ++i) {
            if (condition(this.data[i])) {
                reslist.push(this.data[i]);
                this.data.splice(i, 1);
				i = 0;
            }
        }
        return reslist;
    };
    this.toString = function() {
        if (serializer == null) {
            serializer = function(a) {
                return a + ";";
            }
        }
        var str = "";
        for (var i = 0; i < this.data.length; ++i) {
            str += serializer(this.data[i]);
        }
        return str;
    };
    this.empty = function() {
        return this.data.length == 0;
    };
    this.count = function() {
        return this.data.length;
    }
}