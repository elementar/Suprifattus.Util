/**
 * Encodes a string into the Base64 encoded notation.
 *
 * @param string the string to encode
 * @return string the encoded string
 * @author www.farfarfar.com
 * @author http://rumkin.com/tools/compression/base64.php
 * @version 0.1
 */

function base64Encode(str)
{
	var charBase64 = new Array(
		'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P',
		'Q','R','S','T','U','V','W','X','Y','Z','a','b','c','d','e','f',
		'g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v',
		'w','x','y','z','0','1','2','3','4','5','6','7','8','9','+','/'
	);

	var out = "";
	var chr1, chr2, chr3;
	var enc1, enc2, enc3, enc4;
	var i = 0;

	var len = str.length;

	do
	{
		chr1 = str.charCodeAt(i++);
		chr2 = str.charCodeAt(i++);
		chr3 = str.charCodeAt(i++);

		//enc1 = (chr1 & 0xFC) >> 2;
		enc1 = chr1 >> 2;
		enc2 = ((chr1 & 0x03) << 4) | (chr2 >> 4);
		enc3 = ((chr2 & 0x0F) << 2) | (chr3 >> 6);
		enc4 = chr3 & 0x3F;

		out += charBase64[enc1] + charBase64[enc2];

		if (isNaN(chr2))
  		{
			out += '==';
		}
  		else if (isNaN(chr3))
  		{
			out += charBase64[enc3] + '=';
		}
		else
		{
			out += charBase64[enc3] + charBase64[enc4];
		}
	}
	while (i < len);

	return out;
}

/**
 * Decodes a string from the Base64 encoded notation.
 *
 * @param string the string to decode
 * @return string the decoded string
 * @author www.farfarfar.com
 * @author http://rumkin.com/tools/compression/base64.php
 * @version 0.1
 */

function base64Decode(str)
{
	var indexBase64 = new Array(
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,62, -1,-1,-1,63,
		52,53,54,55, 56,57,58,59, 60,61,-1,-1, -1,-1,-1,-1,
		-1, 0, 1, 2,  3, 4, 5, 6,  7, 8, 9,10, 11,12,13,14,
		15,16,17,18, 19,20,21,22, 23,24,25,-1, -1,-1,-1,-1,
		-1,26,27,28, 29,30,31,32, 33,34,35,36, 37,38,39,40,
		41,42,43,44, 45,46,47,48, 49,50,51,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1,
		-1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1, -1,-1,-1,-1
	);

	var out = "";
	var chr1, chr2, chr3;
	var enc1, enc2, enc3, enc4;
	var i = 0;

	// trim invalid characters in the beginning and in the end of the string

	str = str.replace(/^[^a-zA-Z0-9\+\/\=]+|[^a-zA-Z0-9\+\/\=]+$/g,"")

	var len = str.length;

	do
	{
		enc1 = indexBase64[str.charCodeAt(i++)];
		enc2 = indexBase64[str.charCodeAt(i++)];
		enc3 = indexBase64[str.charCodeAt(i++)];
		enc4 = indexBase64[str.charCodeAt(i++)];

		chr1 = (enc1 << 2) | (enc2 >> 4);
		chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
		chr3 = ((enc3 & 3) << 6) | enc4;

		out += String.fromCharCode(chr1);

		if (enc3 != -1)
		{
			out += String.fromCharCode(chr2);
		}
		if (enc4 != -1)
		{
			out += String.fromCharCode(chr3);
		}
	}
	while (i < len);

	if (i != len)
	{
		return "Possibly invalid Base64 encoded text: " + out;
	}

	return out;
}
