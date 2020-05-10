CREATE TABLE IF NOT EXISTS requirements (
  id INT NOT NULL,
  name VARCHAR(256) NOT NULL,
  doc_number VARCHAR(8) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS methodology (
  id INT NOT NULL,
  name VARCHAR(256) NOT NULL,
  doc_number VARCHAR(8) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS module (
  id INT NOT NULL,
  name VARCHAR(64) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS test_static (
  id INT NOT NULL,
  ts_index VARCHAR(26) NOT NULL,
  mode VARCHAR(16) NOT NULL,
  module INT NOT NULL,
  methodology INT NOT NULL,
  requirements INT NOT NULL,
  unit VARCHAR(8) NULL DEFAULT NULL,
  description VARCHAR(128) NULL DEFAULT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS location (
  id INT NOT NULL,
  name VARCHAR(256) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS staff (
  id INT NOT NULL,
  surname VARCHAR(64) NOT NULL,
  first_name VARCHAR(64) NOT NULL,
  patronymic_name VARCHAR(64) NULL DEFAULT NULL,
  post VARCHAR(16) NOT NULL,
  department VARCHAR(64) NOT NULL,
  login VARCHAR(20) NOT NULL,
  password VARCHAR(64) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS challenge_type (
  id INT NOT NULL,
  name VARCHAR(32) NOT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS controll_object (
  id INT NOT NULL,
  name VARCHAR(64) NOT NULL,
  serial_number VARCHAR(5) NOT NULL,
  decimal_number VARCHAR(32) NOT NULL,
  version VARCHAR(16) NOT NULL,
  parent VARCHAR(8) NULL DEFAULT NULL,
  product VARCHAR(64) NUL DEFAULT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS challenge (
  id INT NOT NULL,
  controll_object INT NOT NULL,
  challenge_type INT NOT NULL,
  staff INT NOT NULL,
  location INT NOT NULL,
  begin_time DATETIME(6) NOT NULL,
  end_time DATETIME(6) NOT NULL,
  description VARCHAR(128) NULL DEFAULT NULL,
  PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS test_dynamic (
  id INT NOT NULL,
  ts_index VARCHAR(26) NOT NULL,
  challenge INT NOT NULL,
  critical_param INT NULL DEFAULT NULL,
  begin_time DATETIME(6) NOT NULL,
  end_time DATETIME(6) NOT NULL,
  nominal DECIMAL(16,6) NULL DEFAULT NULL,
  actual_value DECIMAL(16,6) NULL DEFAULT NULL,
  delta DECIMAL(16,6) NULL DEFAULT NULL,
  boundary_value DECIMAL(16,6) NULL DEFAULT NULL,
  status BOOL NOT NULL,
  PRIMARY KEY (id)
);