s/\[dbo\]\.|(NON)?CLUSTERED|(COLLATE|CONSTRAINT)\s+\[[^]]+]\s+//g;
s/(TEXTIMAGE_)?ON\s+\[?PRIMARY\]?//g;
s/COLLATE \w+//g;
s/(\[|\])//g;
s/getdate\(\)/'NOW\(\)'/g;
s/n((var)?char|text)/\1/g;
s/image/blob/g;
s/money/decimal/g;
s/IDENTITY \([^)]+\)/auto_increment/g;
s/DEFAULT\s+\(([^)]+)\)/DEFAULT \1/g;
s/^GO\s*$/;/
