-- 1. Create Keyspace
CREATE KEYSPACE IF NOT EXISTS my_keyspace
WITH replication = {
  'class': 'SimpleStrategy',
  'replication_factor': 1
};

-- 2. Use the Keyspace
USE my_keyspace;

-- 3. Create a Table (e.g., users)
CREATE TABLE IF NOT EXISTS users (
  user_id UUID PRIMARY KEY,
  first_name TEXT,
  last_name TEXT,
  email TEXT,
  created_at TIMESTAMP
);