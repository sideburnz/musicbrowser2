CREATE TABLE  `d_musicbrowser2`.`t_cache` (
`key` VARCHAR( 64 ) NOT NULL ,
`timestamp` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ,
`value` TEXT NOT NULL ,
PRIMARY KEY (  `key` )
) ENGINE = MYISAM ;