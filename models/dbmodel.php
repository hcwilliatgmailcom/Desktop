<?php

class dbmodel
{
    private $db;
    private $table;

    public function __construct(PDO $db,$table)
    {
        $this->db = $db;
        $this->table = $table;
    }

    public function getAll($page = 1, $pageSize = 10)
    {
        $start = ($page - 1) * $pageSize;

        $stmt = $this->db->prepare("SELECT * FROM ".$this->table." LIMIT :start, :pageSize");
        $stmt->bindParam(':start', $start, PDO::PARAM_INT);
        $stmt->bindParam(':pageSize', $pageSize, PDO::PARAM_INT);
        $stmt->execute();

        return $stmt->fetchAll(PDO::FETCH_ASSOC);
    }

    public function getTotal() 
    {
        $stmt = $this->db->query("SELECT COUNT(*) FROM ".$this->table); 
        return $stmt->fetchColumn();
    }
    
    public function getById($id)
    {
        $stmt = $this->db->prepare("SELECT * FROM ".$this->table." WHERE id = " . $id);
 
        $stmt->execute();

        return $stmt->fetch(PDO::FETCH_ASSOC);
    }
}