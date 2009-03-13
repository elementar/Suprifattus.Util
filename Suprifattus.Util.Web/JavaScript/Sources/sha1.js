function sha1(s){
	function X(x,y){var l=(x&0xFFFF)+(y&0xFFFF),m=(x>>16)+(y>>16)+(l>>16);return(m<<16)|(l&0xFFFF)}
	function Y(x,y){return(x<<y)|(x>>>(32-y))}
	var len=s.length*8,i,L=((len+64>>9)<<4)+16,x=Array(L+79),w=Array(80),a=1732584193,b=-271733879,c=-1732584194,d=271733878,e=-1009589776;
	for(i=0;i<x.length;++i)x[i]=0;
	for(i=0;i<len;i+=8)x[i>>5]|=(s.charCodeAt(i/8)&255)<<(24-i%32);
	x[len>>5]|=0x80<<(24-len%32);
	x[L-1]=len;
	for(i=0;i<L;i+=16){
		var oa=a,ob=b,oc=c,od=d,oe=e;
		for(var j=0;j<80;j++){
			w[j]=(j<16)?x[i+j]:Y(w[j-3]^w[j-8]^w[j-14]^w[j-16],1);
			var t=X(X(Y(a,5),((j<20)?((b&c)|((~b)&d)):((j<40||j>=60)?(b^c^d):((b&c)|(b&d)|(c&d))))),X(X(e,w[j]),((j<20)?1518500249:(j<40)?1859775393:(j<60)?-1894007588:-899497514)));
			e=d;
			d=c;
			c=Y(b,30);
			b=a;
			a=t;
		}
		a=X(a,oa);
		b=X(b,ob);
		c=X(c,oc);
		d=X(d,od);
		e=X(e,oe);
	}
	x=[a,b,c,d,e];
	a="0123456789abcdef";
	b="";
	for(i=0;i<20;i++)b+=a.charAt((x[i>>2]>>((3-i%4)*8+4))&0xF)+a.charAt((x[i>>2]>>((3-i%4)*8))&0xF);
	return b
}
