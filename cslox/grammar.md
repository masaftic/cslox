program -> declaration* EOF

declaration -> var_decl | function_decl | class_decl | statement   

var_decl -> "var" IDENTIFIER ("=" expression)? ";"

function_decl -> "fun" function

class_decl -> "class" IDENTIFIER ("<" IDENTIFIER)? "{" function* "}"

function -> IDENTIFIER "(" parameters? ")" block

parameters -> IDENTIFIER ("," IDENTIFIER)*

statement -> block
       | print_stmt
       | if_stmt
       | while_stmt
       | for_stmt
       | return_stmt
       | expr_stmt

block -> "{" declaration* "}"

print_stmt -> "print" expression ";"

if_stmt -> "if" "(" expression ")" statement ("else" statement)?

while_stmt -> "while" "(" expression ")" statement

for_stmt -> "for" "(" (var_decl | statement | ";")
        expression? ";" expression? ")" statement

        
return_stmt -> "return" expression? ";"

expr_stmt -> expression ";"

expression -> assignment

assignment -> (call ".")? IDENTIFIER "=" assignment | logical_or

logical_or -> logical_and ("or" logical_and)*

logical_and -> equality ("and" equality)*

equality -> comparison (("==" | "!=") comparison)*

comparison -> term ((">" | ">=" | "<" | "<=") term)*

term -> factor (("+" / "-") factor)*

factor -> unary (("*" / "/") unary)*

unary -> ("-" | "!") unary | power

power -> call ** power | call

call -> primary ("(" arguments? ")")*

arguments -> expression ("," expression)*

primary -> INTEGER
     | FLOAT
     | STRING
     | "true"
     | "false"
     | "nil"
     | "(" expression ")"
     | IDENTIFIER
     | "super" "." IDENTIFIER
