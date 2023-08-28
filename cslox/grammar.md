expression -> literal
            | unary
            | binary
            | grouping ;

literal    -> NUMBER | STRING | "ture" | "false" | "nil" ;
grouping   -> "(" expression ")" ;
unary      -> ( "-" | "!" ) expression ;
binary     -> expression operator expression ;
operator   -> "==" | "!=" | "<" | "<=" | "<" | "<="
            | "+" | "-" | "*" | "/" ;
