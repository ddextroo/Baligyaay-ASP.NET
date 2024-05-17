CREATE TABLE product(
    prod_id INT IDENTITY(1, 1) PRIMARY KEY,
    prod_name VARCHAR(255) NOT NULL,
    prod_description TEXT NOT NULL,
    prod_price NUMERIC(10, 2) NOT NULL,
    prod_stock INT NOT NULL,
    prod_img_url TEXT NOT NULL
);

CREATE TABLE product_char(
    char_id INT IDENTITY(1, 1) PRIMARY KEY,
    char_material VARCHAR(255) NOT NULL,
    char_length NUMERIC(9, 2) NOT NULL,
    char_width NUMERIC(9, 2) NOT NULL,
    char_height NUMERIC(9, 2) NOT NULL,
    char_weight NUMERIC(10, 2) NOT NULL,
    prod_id INT REFERENCES product NOT NULL
) CREATE TABLE category(
    cat_id INT IDENTITY(1, 1) PRIMARY KEY,
    cat_name VARCHAR(255) NOT NULL,
    cat_description TEXT NOT NULL
)
ALTER TABLE
    product
ADD
    cat_id INT REFERENCES category ON UPDATE CASCADE ON DELETE CASCADE NOT NULL
ALTER TABLE
    product
ADD
    cat_id INT REFERENCES category ON UPDATE CASCADE ON DELETE CASCADE NOT NULL;

CREATE TABLE customer(
    cus_id INT IDENTITY(1, 1) PRIMARY KEY,
    cus_fname VARCHAR(255) NOT NULL,
    cus_lname VARCHAR(255) NOT NULL,
    cus_email VARCHAR(100) NOT NULL UNIQUE,
    cus_password VARCHAR(255) NOT NULL,
    cus_phone VARCHAR(255)
) CREATE TABLE cus_order(
    order_id INT IDENTITY(1, 1) PRIMARY KEY,
    order_date TIMESTAMP NOT NULL,
    order_status VARCHAR(50) NOT NULL,
    order_total_price NUMERIC(9, 2) NOT NULL,
    cus_id INT REFERENCES customer ON UPDATE CASCADE ON DELETE CASCADE NOT NULL
) CREATE TABLE order_items(
    order_item_id INT IDENTITY(1, 1) PRIMARY KEY,
    order_item_quantity INT NOT NULL,
    order_item_price NUMERIC(9, 2) NOT NULL,
    cus_id INT REFERENCES customer NOT NULL,
    prod_id INT REFERENCES product NOT NULL,
    order_id INT REFERENCES cus_order ON UPDATE CASCADE ON DELETE CASCADE NOT NULL
)
SELECT
    *
FROM
    category
DELETE FROM
    product_char
SELECT
    product.prod_id,
    product.prod_name,
    product.prod_description,
    product.prod_price,
    product.prod_stock,
    product.prod_img_url,
    category.cat_id,
    category.cat_name,
    product_char.char_id,
    product_char.char_material,
    product_char.char_length,
    product_char.char_width,
    product_char.char_height,
    product_char.char_weight
FROM
    product
    INNER JOIN category ON product.cat_id = category.cat_id
    INNER JOIN product_char ON product.prod_id = product_char.prod_id
DELETE FROM
    category